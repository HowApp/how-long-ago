namespace How.Core.CQRS.Queries.General.CheckExistForUSer;

using CheckExistForUser;
using Dapper;
using Database;
using Database.Entities.Base;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
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
            var query = $@"
SELECT EXISTS(
    SELECT 1 FROM {request.Table} 
    WHERE 
    {nameof(BaseCreeated.Id).ToSnake()} = @id AND
    {nameof(BaseCreeated.CreatedById).ToSnake()} = @created_by_id);
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