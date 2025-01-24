namespace How.Core.CQRS.Commands.Storage.DeleteImageMultiply;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;

public class DeleteImageMultiplyCommandHandler : ICommandHandler<DeleteImageMultiplyCommand, Result>
{
    private readonly ILogger<DeleteImageMultiplyCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteImageMultiplyCommandHandler(ILogger<DeleteImageMultiplyCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result> Handle(DeleteImageMultiplyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var removeImageSql = @$"
DELETE FROM {nameof(BaseDbContext.StorageImages).ToSnake()}
WHERE {nameof(StorageImage.Id).ToSnake()} = ANY(@imageId)
RETURNING {nameof(StorageImage.MainId).ToSnake()}, {nameof(StorageImage.ThumbnailId).ToSnake()};
";
            await using var connection = _dapper.InitConnection();
            var oldFiles = await connection.QueryAsync<(int,int)>(
                removeImageSql, new
                {
                    imageId = request.ImageIds
                });
            var sourceArr = oldFiles.ToArray();

            var length = sourceArr.Length;
            var combinedArray = new int[length * 2];
            Array.Copy(sourceArr.Select(a => a.Item1).ToArray(), combinedArray, length);
            Array.Copy(sourceArr.Select(a => a.Item2).ToArray(), 0, combinedArray, length, length);
            
            var removeFileSql = $@"
DELETE FROM {nameof(BaseDbContext.StorageFiles).ToSnake()}
WHERE {nameof(StorageFile.Id).ToSnake()} = ANY(@imageId);
";
            await connection.ExecuteAsync(removeFileSql, new
            {
                imageId = combinedArray
            });
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Storage, $"Error while executing {nameof(DeleteImageMultiplyCommand)}"));
        }
    }
}