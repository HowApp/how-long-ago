namespace How.Core.Infrastructure.Builders;

using System.Text;
using Common.Extensions;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.SharedUser;
using Enums;

public class EventAccessQueryBuilder : IEventAccessQueryBuilder
{
    private StringBuilder Query { get; } = new StringBuilder();
    private DynamicParameters Parameters { get; } = new DynamicParameters();

    public EventAccessQueryBuilder()
    {
        Query.Append($@"SELECT 1 WHERE 1 = 0");
    }

    public void Init(int recordId)
    {
        Query.Clear();
        
        Query.Append($@"
SELECT 1 FROM {nameof(BaseDbContext.Events).ToSnake()} e
    WHERE e.{nameof(Event).ToSnake()} = FALSE AND
    {nameof(BaseCreated.Id).ToSnake()} = @recordId
");
        Parameters.Add("@recordId", recordId);
    }

    public void FilterCreatedBy(int userId, bool shared = false)
    {
        Parameters.Add("@createdById", userId);

        if (shared)
        {
            Query.Append($@"
    AND
    ({nameof(BaseCreated.CreatedById).ToSnake()} = @createdById
         OR
    EXISTS(
        SELECT 1 
        FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su 
        WHERE 
            su.{nameof(SharedUser.UserOwnerId).ToSnake()} = t.{nameof(BaseCreated.CreatedById).ToSnake()} 
          AND 
            su.{nameof(SharedUser.UserSharedId).ToSnake()} = @createdById)
        )
");
        }
        else
        {
            Query.Append($@"
    AND
    {nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id
");
        }
    }

    public void FilterByEventStatus(EventStatus status)
    {
        Parameters.Add("@eventStatus", status);
        
        Query.Append($@"
    AND
    e.{nameof(Event.Status).ToSnake()} = @eventStatus
");
    }

    public void FilterByEventAccessType(EventAccessType accessType)
    {
        Parameters.Add("@accessType", accessType);
        
        Query.Append($@"
    AND
    e.{nameof(Event.Access).ToSnake()} = @accessType
");
    }

    public (string sqlQuery, DynamicParameters paramsQuery) BuildQuery()
    {
        var query = $@"
SELECT EXISTS({Query});
";

        return (query, Parameters);
    }
}