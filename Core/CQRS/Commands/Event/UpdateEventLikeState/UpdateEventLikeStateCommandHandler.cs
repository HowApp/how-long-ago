namespace How.Core.CQRS.Commands.Event.UpdateEventLikeState;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Microsoft.Extensions.Logging;

public class UpdateEventLikeStateCommandHandler : ICommandHandler<UpdateEventLikeStateCommand, Result<int>>
{
    private readonly ILogger<UpdateEventLikeStateCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateEventLikeStateCommandHandler(ILogger<UpdateEventLikeStateCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateEventLikeStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO {nameof(BaseDbContext.LikedEvents).ToSnake()} (
                                           {nameof(LikedEvent.EventId).ToSnake()},
                                           {nameof(LikedEvent.LikedByUserId).ToSnake()},
                                           {nameof(LikedEvent.State).ToSnake()}
)
VALUES (@eventId, @likedByUserId, @state)
ON CONFLICT ({nameof(LikedEvent.EventId).ToSnake()}, {nameof(LikedEvent.LikedByUserId).ToSnake()})
DO UPDATE SET 
              {nameof(LikedEvent.State).ToSnake()} = @state;

SELECT *
FROM {nameof(BaseDbContext.LikedEvents).ToSnake()} le 
WHERE le.{nameof(LikedEvent.LikedByUserId).ToSnake()} = @likedByUserId AND le.{nameof(LikedEvent.EventId).ToSnake()} = @eventId;
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    eventId = request.EventId,
                    likedByUserId = request.CurrentUserId,
                    state = request.LikeState
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Event, $"Error while executing {nameof(UpdateEventLikeStateCommand)}"));
        }
    }
}