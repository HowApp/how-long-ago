namespace How.Core.CQRS.Queries.General.CheckExist;

using Dapper;
using Database;
using Database.Entities.Base;
using Common.CQRS;
using HowCommon.Extensions;
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
            var usersTable = nameof(BaseDbContext.Users).ToSnake();
            var eventsTable = nameof(BaseDbContext.Events).ToSnake();
            var recordsTable = nameof(BaseDbContext.Records).ToSnake();

            string tableName;
            string pkColumn;

            if (string.Equals(request.Table, usersTable, StringComparison.OrdinalIgnoreCase))
            {
                tableName = usersTable;
                pkColumn = "user_id";
            }
            else if (string.Equals(request.Table, eventsTable, StringComparison.OrdinalIgnoreCase))
            {
                tableName = eventsTable;
                pkColumn = nameof(PKey.Id).ToSnake();
            }
            else if (string.Equals(request.Table, recordsTable, StringComparison.OrdinalIgnoreCase))
            {
                tableName = recordsTable;
                pkColumn = nameof(PKey.Id).ToSnake();
            }
            else
            {
                throw new ArgumentException($"Unsupported table in {nameof(CheckExistQuery)}: {request.Table}");
            }

            var query = $@"
SELECT EXISTS(SELECT 1 FROM {tableName} WHERE {pkColumn} = @id);
";
            await using var connection = _dapper.InitConnection();

            var result = await connection.QuerySingleAsync<bool>(
                query,
                new
                {
                    id = request.Id
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