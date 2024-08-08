namespace How.Core.CQRS.Queries.Event.CheckAccessToEvent;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.SharedUser;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;

public class CheckAccessToEventQueryHandler : IQueryHandler<CheckAccessToEventQuery, Result<bool>>
{
    private readonly ILogger<CheckAccessToEventQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckAccessToEventQueryHandler(ILogger<CheckAccessToEventQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckAccessToEventQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT EXISTS(
    SELECT 1
    FROM {nameof(BaseDbContext.Events).ToSnake()} e
    WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE AND
          e.{nameof(IdentityKey.Id).ToSnake()} = @eventId AND 
          e.{nameof(Event.Status).ToSnake()} = {(int)EventStatus.Active} AND
          (e.{nameof(Event.Access).ToSnake()} = {(int)EventAccessType.Public} OR
           ({nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id OR
            EXISTS(
                         SELECT 1
                         FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su
                         WHERE
                             su.{nameof(SharedUser.UserOwnerId).ToSnake()} = e.{nameof(BaseCreated.CreatedById).ToSnake()} AND
                             su.{nameof(SharedUser.UserSharedId).ToSnake()} = @created_by_id))
              )
    );
";
            
            await using var connection = _dapper.InitConnection();

            var result = await connection.QuerySingleAsync<bool>(
                query,
                new
                {
                    created_by_id = request.CurrentUserId,
                    eventId = request.EventId
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<bool>(
                new Error(ErrorType.Event, $"Error while executing {nameof(CheckAccessToEventQuery)}"));
        }
    }
}