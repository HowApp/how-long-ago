namespace How.Core.CQRS.Queries.General.CheckExistAccess;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Microsoft.Extensions.Logging;

public class CheckExistAccessQueryHandler : IQueryHandler<CheckExistAccessQuery, Result<bool>>
{
    private readonly ILogger<CheckExistAccessQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckExistAccessQueryHandler(ILogger<CheckExistAccessQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckExistAccessQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.QueryAccessBuilder is null)
            {
                return Result.Failure<bool>(
                    new Error(ErrorType.QueryBuilder, $"Query builder is NULL!"));
            }
            var queryData = request.QueryAccessBuilder.BuildQuery();
            
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
                new Error(ErrorType.QueryBuilder, $"Error while executing {nameof(CheckExistAccessQuery)}"));
        }
    }
}