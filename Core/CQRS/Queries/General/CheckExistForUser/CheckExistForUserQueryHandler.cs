namespace How.Core.CQRS.Queries.General.CheckExistForUSer;

using CheckExistForUser;
using Dapper;
using Database;
using Database.Entities.Base;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Database.Entities.SharedUser;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;

public class CheckExistForUserQueryHandler : IQueryHandler<CheckExistForUserQuery, Result<bool>>
{
    private readonly ILogger<CheckExistForUserQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckExistForUserQueryHandler(ILogger<CheckExistForUserQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckExistForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var innerQuery = string.Empty; 

            switch (request.FilterType)
            {
                case FilterType.CreatedBy:
                    innerQuery = $@"
SELECT 1 FROM {request.Table} 
    WHERE 
    {nameof(BaseCreated.Id).ToSnake()} = @id
    AND
    {nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id
";
                    break;
                case FilterType.IncludeShared:
                    innerQuery = $@"
SELECT 1 FROM {request.Table} t
    WHERE 
    t.{nameof(BaseCreated.Id).ToSnake()} = @id
    AND
    ({nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id
         OR
    EXISTS(
        SELECT 1 
        FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su 
        WHERE 
            su.{nameof(SharedUser.UserOwnerId).ToSnake()} = t.{nameof(BaseCreated.CreatedById).ToSnake()} 
          AND 
            su.{nameof(SharedUser.UserSharedId).ToSnake()} = @created_by_id)
        )
";
                    break;
                default:
                    return Result.Success(false);
                    break;
            }
                
            var query = $@"
SELECT EXISTS({innerQuery});
";
            await using var connection = _dapper.InitConnection();

            var result = await connection.QuerySingleAsync<bool>(
                query,
                new
                {
                    created_by_id = request.CurrentUserId,
                    id = request.Id,
                    table_name = request.Table
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<bool>(
                new Error(ErrorType.Record, $"Error while executing {nameof(CheckExistForUserQuery)}"));
        }
    }
}