namespace How.Core.CQRS.Commands.Event.UpdateEventAccess;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Microsoft.Extensions.Logging;
using NodaTime;

public class UpdateEventAccessCommandHandler : ICommandHandler<UpdateEventAccessCommand, Result<int>>
{
    private readonly ILogger<UpdateEventAccessCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateEventAccessCommandHandler(ILogger<UpdateEventAccessCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateEventAccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
UPDATE {nameof(BaseDbContext.Events).ToSnake()}
SET 
    {nameof(Event.Access).ToSnake()} = @access,
    {nameof(Event.ChangedById).ToSnake()} = @changed_by_id,
    {nameof(Event.ChangedAt).ToSnake()} = @changed_at
WHERE 
    {nameof(Event.Id).ToSnake()} = @id AND
    {nameof(Event.CreatedById).ToSnake()} = @created_by_id
RETURNING *;
";
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command, new
                {
                    access = request.Access,
                    changed_by_id = request.CurrentUserId,
                    changed_at = SystemClock.Instance.GetCurrentInstant(),
                    id = request.EventId,
                    created_by_id = request.CurrentUserId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Event, $"Error while executing {nameof(UpdateEventAccessCommand)}"));
        }
    }
}