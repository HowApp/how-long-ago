namespace How.Core.CQRS.Queries.General.CheckExist;

using Dapper;
using Database;
using Database.Entities.Base;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Microsoft.Extensions.Logging;

public class CheckExistQueryHandler : IQueryHandler<CheckExistQuery, Result<bool>>
{
    private readonly ILogger<CheckExistQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckExistQueryHandler(ILogger<CheckExistQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckExistQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT EXISTS(SELECT 1 FROM {request.Table} WHERE {nameof(BaseIdentityKey.Id).ToSnake()} = @id);
";
            await using var connection = _dapper.InitConnection();
            
            var result = await connection.QuerySingleAsync<bool>(
                query,
                new
                {
                    id = request.Id,
                    table_name = request.Table
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<bool>(
                new Error(ErrorType.Record, $"Error while executing {nameof(CheckExistQuery)}"));
        }
    }
}