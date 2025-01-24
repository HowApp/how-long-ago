namespace How.Core.CQRS.Commands.Event.DeleteEventFromSaved;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Microsoft.Extensions.Logging;

public class DeleteEventFromSavedCommandHandler : ICommandHandler<DeleteEventFromSavedCommand, Result<int>>
{
    private readonly ILogger<DeleteEventFromSavedCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteEventFromSavedCommandHandler(ILogger<DeleteEventFromSavedCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(DeleteEventFromSavedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
DELETE FROM {nameof(BaseDbContext.SavedEvents).ToSnake()}
WHERE {nameof(SavedEvent.EventId).ToSnake()} = @eventId AND
      {nameof(SavedEvent.UserId).ToSnake()} = @userId
RETURNING *;
";
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command, new
                {
                    eventId = request.EventId,
                    userId = request.CurrentUserId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Event, $"Error while executing {nameof(DeleteEventFromSavedCommand)}"));
        }
    }
}