namespace How.Core.Infrastructure.Builders;

using System.Text;
using Common.Extensions;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.SharedUser;
using Enums;

public class EventAccessQueryAccessBuilder : IEventAccessQueryAccessBuilder
{
    private readonly StringBuilder _query = new StringBuilder();
    private readonly DynamicParameters _parameters = new DynamicParameters();

    public EventAccessQueryAccessBuilder(int eventId)
    {
        _query.Append($@"SELECT 1 FROM {nameof(BaseDbContext.Events).ToSnake()} e WHERE 1 = 0");

        if (eventId >= 0)
        {
            Init(eventId);
        }
    }

    public void Init(int eventId)
    {
        _query.Clear();
        
        _query.Append($@"
SELECT 1 FROM {nameof(BaseDbContext.Events).ToSnake()} e
    WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE AND
    e.{nameof(BaseCreated.Id).ToSnake()} = @eventId
");
        _parameters.Add("@eventId", eventId);
    }

    public void FilterCreatedBy(int userId, AccessFilterType accessFilterType = AccessFilterType.IncludeCreatedBy)
    {
        _parameters.Add("@createdById", userId);

        switch (accessFilterType)
        {
            case AccessFilterType.IncludeCreatedBy:
                _query.Append($@"
    AND
    e.{nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id
");
                break;
            case AccessFilterType.IncludeShared:
                _query.Append($@"
    AND
    (e.{nameof(BaseCreated.CreatedById).ToSnake()} = @createdById
         OR
    EXISTS(
        SELECT 1 
        FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su 
        WHERE 
            su.{nameof(SharedUser.UserOwnerId).ToSnake()} = e.{nameof(BaseCreated.CreatedById).ToSnake()} 
          AND 
            su.{nameof(SharedUser.UserSharedId).ToSnake()} = @createdById)
        )
");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(accessFilterType), accessFilterType, null);
        }
    }

    public void FilterByStatus(EventStatus status)
    {
        _parameters.Add("@eventStatus", status);
        
        _query.Append($@"
    AND
    e.{nameof(Event.Status).ToSnake()} = @eventStatus
");
    }

    public void FilterByAccessType(EventAccessType accessType)
    {
        _parameters.Add("@accessType", accessType);
        
        _query.Append($@"
    AND
    e.{nameof(Event.Access).ToSnake()} = @accessType
");
    }

    public (string sqlQuery, DynamicParameters paramsQuery) BuildQuery()
    {
        var query = $@"
SELECT EXISTS({_query});
";

        return (query, _parameters);
    }
}