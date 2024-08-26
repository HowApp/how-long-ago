namespace How.Core.CQRS.Queries.Event.CheckEvent;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Microsoft.Extensions.Logging;

public class CheckEventQueryHandler : IQueryHandler<CheckEventQuery, Result<bool>>
{
    private readonly ILogger<CheckEventQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckEventQueryHandler(ILogger<CheckEventQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckEventQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var queryData = request.QueryBuilder.BuildQuery();
            
            await using var connection = _dapper.InitConnection();

            var result = await connection.QuerySingleAsync<bool>(
                queryData.sqlQuery,
                queryData.paramsQuery);

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<bool>(
                new Error(ErrorType.Event, $"Error while executing {nameof(CheckEventQuery)}"));
        }
    }
}