namespace How.Core.CQRS.Commands.Event.AddEventToSaved;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using HowCommon.Extensions;
using Microsoft.Extensions.Logging;

public class AddEventToSavedCommandHandler : ICommandHandler<AddEventToSavedCommand, Result<int>>
{
    private readonly ILogger<AddEventToSavedCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public AddEventToSavedCommandHandler(ILogger<AddEventToSavedCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(AddEventToSavedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO {nameof(BaseDbContext.SavedEvents).ToSnake()} (
    {nameof(SavedEvent.EventId).ToSnake()},
    {nameof(SavedEvent.UserId).ToSnake()}
)
VALUES (@eventId, @userId)
ON CONFLICT ({nameof(SavedEvent.EventId).ToSnake()}, {nameof(SavedEvent.UserId).ToSnake()})
DO NOTHING;

SELECT * 
FROM {nameof(BaseDbContext.SavedEvents).ToSnake()} se
WHERE se.{nameof(SavedEvent.EventId).ToSnake()} = @eventId AND 
      se.{nameof(SavedEvent.UserId).ToSnake()} = @userId;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    userId = request.CurrentUserId,
                    eventId = request.EventId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Event, $"Error while executing {nameof(AddEventToSavedCommand)}"));
        }
    }
}