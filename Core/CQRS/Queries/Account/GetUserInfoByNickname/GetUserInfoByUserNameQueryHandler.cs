namespace How.Core.CQRS.Queries.Account.GetUserInfoByNickname;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Identity;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;

public class GetUserInfoByUserNameQueryHandler : IQueryHandler<GetUserInfoByUserNameQuery, Result<List<GetUserInfoByUserNameQueryResult>>>
{
    private readonly ILogger<GetUserInfoByUserNameQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetUserInfoByUserNameQueryHandler(ILogger<GetUserInfoByUserNameQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<List<GetUserInfoByUserNameQueryResult>>> Handle(
        GetUserInfoByUserNameQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    u.{nameof(HowUser.Id).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.Id)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.FirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.LastName)},
    u.{nameof(HowUser.UserName).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.UserName)},
    main_image.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.MainHash)},
    thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetUserInfoByUserNameQueryResult.ThumbnailHash)}
FROM {nameof(BaseDbContext.Users).ToSnake()} u
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} si ON u.{nameof(HowUser.StorageImageId).ToSnake()} = si.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} main_image ON main_image.id = si.{nameof(StorageImage.MainId).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} thumbnail on thumbnail.id = si.{nameof(StorageImage.ThumbnailId).ToSnake()}
WHERE u.{nameof(HowUser.UserName).ToSnake()} ILIKE '%' || @search || '%'
LIMIT 50;
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.QueryAsync<GetUserInfoByUserNameQueryResult>(
                query,
                new { search = request.Search });
            
            return Result.Success(result.ToList());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<List<GetUserInfoByUserNameQueryResult>>(
                new Error(ErrorType.Account, $"Error while executing {nameof(GetUserInfoByUserNameQuery)}"));
        }
    }
}