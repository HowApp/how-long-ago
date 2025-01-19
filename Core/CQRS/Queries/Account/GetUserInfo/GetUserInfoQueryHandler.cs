namespace How.Core.CQRS.Queries.Account.GetUserInfo;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;

public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, Result<GetUserInfoQueryResult>>
{
    private readonly ILogger<GetUserInfoQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetUserInfoQueryHandler(ILogger<GetUserInfoQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetUserInfoQueryResult>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
           var query = $@"
SELECT 
    u.{nameof(HowUser.UserId).ToSnake()} AS {nameof(GetUserInfoQueryResult.Id)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(GetUserInfoQueryResult.FirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(GetUserInfoQueryResult.LastName)},
    main_image.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetUserInfoQueryResult.MainHash)},
    thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetUserInfoQueryResult.ThumbnailHash)}
FROM {nameof(BaseDbContext.Users).ToSnake()} u
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} si ON u.{nameof(HowUser.StorageImageId).ToSnake()} = si.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} main_image ON main_image.id = si.{nameof(StorageImage.MainId).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} thumbnail on thumbnail.id = si.{nameof(StorageImage.ThumbnailId).ToSnake()}
WHERE u.{nameof(HowUser.UserId).ToSnake()} = @userId
LIMIT 1
";

           await using var connection = _dapper.InitConnection();
           var result = await connection.QueryFirstOrDefaultAsync<GetUserInfoQueryResult>(
               query,
               new { userId = request.CurrentUserId });

           
           if (result is null)
           {
               return Result.Failure<GetUserInfoQueryResult>(
                   new Error(ErrorType.Account, "User not found!"), 404);
           }
           
           return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetUserInfoQueryResult>(
                new Error(ErrorType.Account, $"Error while executing {nameof(GetUserInfoQuery)}"));
        }
    }
}