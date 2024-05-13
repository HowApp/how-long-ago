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
        try
        {
            var updateImageSql = $@"
UPDATE users
SET storage_image_id = @imageId
WHERE id = @userId
RETURNING (SELECT u.storage_image_id FROM users U WHERE U.id = @userId)
";
            await using var connection = _dapper.InitConnection();
            
            var oldImageId = await connection.QueryFirstOrDefaultAsync<int>(
                updateImageSql, new
                {
                    imageId = request.ImageId,
                    userId = request.CurrentUserId
                });

            if (oldImageId != default)
            {
                var removeImageSql = @"
DELETE FROM storage_images
WHERE id = @imageId
RETURNING (SELECT i.main_id, i.thumbnail_id FROM storage_images i WHERE i.id = @imageId)
";
                var oldFiles = await connection.QueryAsync<int>(
                    removeImageSql, new
                    {
                        imageId = oldImageId
                    });

                var removeFileSql = @"
DELETE FROM storage_files
WHERE id = ANY(@imageId)
";
                await connection.ExecuteAsync(removeFileSql, new
                {
                    imageId = oldFiles
                });
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Account, $"Error while executing {nameof(UpdateUserImageCommand)}"));
        }
    }
}