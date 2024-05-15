namespace How.Core.CQRS.Commands.Account.UpdateUserImage;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Microsoft.Extensions.Logging;

public class UpdateUserImageCommandHandler : ICommandHandler<UpdateUserImageCommand, Result>
{
    private readonly ILogger<UpdateUserImageCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateUserImageCommandHandler(ILogger<UpdateUserImageCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result> Handle(
        UpdateUserImageCommand request, 
        CancellationToken cancellationToken)
    {
        await using var connection = _dapper.InitConnection();
        await using var transaction = await connection.BeginTransactionAsync(CancellationToken.None);
        
        try
        {
            var updateImageSql = $@"
UPDATE users
SET storage_image_id = @imageId
WHERE id = @userId
RETURNING (SELECT coalesce(u.storage_image_id, 0) FROM users u WHERE u.id = @userId);
";
            
            var oldImageId = await connection.QueryFirstOrDefaultAsync<int>(
                updateImageSql, new
                {
                    imageId = request.ImageId,
                    userId = request.CurrentUserId
                },
                transaction);

            if (oldImageId != 0)
            {
                var removeImageSql = @"
DELETE FROM storage_images
WHERE id = @imageId
RETURNING main_id, thumbnail_id;
";
                var oldFiles = await connection.QueryFirstOrDefaultAsync<(int,int)>(
                    removeImageSql, new
                    {
                        imageId = oldImageId
                    },
                    transaction);

                var removeFileSql = @"
DELETE FROM storage_files
WHERE id = ANY(@imageId);
";
                await connection.ExecuteAsync(removeFileSql, new
                {
                    imageId = new int[] {oldFiles.Item1, oldFiles.Item2 }
                },
                transaction);
            }
            
            await transaction.CommitAsync(CancellationToken.None);
            return Result.Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Account, $"Error while executing {nameof(UpdateUserImageCommand)}"));
        }
    }
}