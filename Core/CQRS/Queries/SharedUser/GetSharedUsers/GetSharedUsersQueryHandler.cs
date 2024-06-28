namespace How.Core.CQRS.Queries.SharedUser.GetSharedUsers;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using Database.Entities.SharedUser;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;

public class GetSharedUsersQueryHandler : IQueryHandler<GetSharedUsersQuery, Result<List<GetSharedUsersQueryResult>>>
{
    private readonly ILogger<GetSharedUsersQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetSharedUsersQueryHandler(ILogger<GetSharedUsersQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<List<GetSharedUsersQueryResult>>> Handle(
        GetSharedUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    u.{nameof(HowUser.Id).ToSnake()} AS {nameof(GetSharedUsersQueryResult.Id)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(GetSharedUsersQueryResult.FirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(GetSharedUsersQueryResult.LastName)},
    u.{nameof(HowUser.UserName).ToSnake()} AS {nameof(GetSharedUsersQueryResult.UserName)},
    main_image.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetSharedUsersQueryResult.MainHash)},
    thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetSharedUsersQueryResult.ThumbnailHash)}
FROM {nameof(BaseDbContext.Users).ToSnake()} u
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} si ON u.{nameof(HowUser.StorageImageId).ToSnake()} = si.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} main_image ON main_image.id = si.{nameof(StorageImage.MainId).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} thumbnail on thumbnail.id = si.{nameof(StorageImage.ThumbnailId).ToSnake()}
WHERE EXISTS(
    SELECT 1 
    FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su 
    WHERE su.{nameof(SharedUser.UserOwnerId).ToSnake()} = @userId
    );
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.QueryAsync<GetSharedUsersQueryResult>(
                query,
                new { userId = request.CurrentUserId });
            
            return Result.Success(result.ToList());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<List<GetSharedUsersQueryResult>>(
                new Error(ErrorType.Account, $"Error while executing {nameof(GetSharedUsersQuery)}"));
        }
    }
}