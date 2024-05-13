namespace How.Core.CQRS.Commands.Storage.DeleteImage;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
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
            var removeImageSql = @"
DELETE FROM storage_images
WHERE id = @imageId
RETURNING (SELECT i.main_id, i.thumbnail_id FROM storage_images i WHERE i.id = @imageId)
";
            await using var connection = _dapper.InitConnection();
            var oldFiles = await connection.QueryAsync<int>(
                removeImageSql, new
                {
                    imageId = request.ImageId
                });
            
            var removeFileSql = @"
DELETE FROM storage_files
WHERE id = ANY(@imageId)
";
            await connection.ExecuteAsync(removeFileSql, new
            {
                imageId = oldFiles
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