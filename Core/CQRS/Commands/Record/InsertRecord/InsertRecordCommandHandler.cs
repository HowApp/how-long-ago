namespace How.Core.CQRS.Commands.Record.InsertRecord;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Microsoft.Extensions.Logging;
using NodaTime;

public class InsertRecordCommandHandler : ICommandHandler<InsertRecordCommand, Result<int>>
{
    private readonly ILogger<InsertRecordCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InsertRecordCommandHandler(ILogger<InsertRecordCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(InsertRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO {nameof(BaseDbContext.Records).ToSnake()} (
    {nameof(Record.EventId).ToSnake()},                                   
    {nameof(Record.Description).ToSnake()},
    {nameof(Record.CreatedById).ToSnake()},
    {nameof(Record.CreatedAt).ToSnake()}
)
VALUES (@event_id, @description, @created_by_id, @created_at)
RETURNING {nameof(Record.Id).ToSnake()}
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    event_id = request.EventId,
                    description = request.Description,
                    created_by_id = request.CurrentUserId,
                    created_at = SystemClock.Instance.GetCurrentInstant()
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error while executing {nameof(InsertRecordCommand)}"));
        }
    }
}