namespace How.Core.CQRS.Commands.Storage.DeleteImage;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;

public class DeleteImageCommandHandler : ICommandHandler<DeleteImageCommand, Result>
{
    private readonly ILogger<DeleteImageCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteImageCommandHandler(ILogger<DeleteImageCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var removeImageSql = @$"
DELETE FROM {nameof(BaseDbContext.StorageImages).ToSnake()}
WHERE {nameof(StorageImage.Id).ToSnake()} = @imageId
RETURNING {nameof(StorageImage.MainId).ToSnake()}, {nameof(StorageImage.ThumbnailId).ToSnake()};
";
            await using var connection = _dapper.InitConnection();
            var oldFiles = await connection.QueryFirstOrDefaultAsync<(int,int)>(
                removeImageSql, new
                {
                    imageId = request.ImageId
                });
            
            var removeFileSql = $@"
DELETE FROM {nameof(BaseDbContext.StorageFiles).ToSnake()}
WHERE {nameof(StorageFile.Id).ToSnake()} = ANY(@imageId);
";
            await connection.ExecuteAsync(removeFileSql, new
            {
                imageId = new int[] {oldFiles.Item1, oldFiles.Item2 }
            });
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Storage, $"Error while executing {nameof(DeleteImageCommand)}"));
        }
    }
}