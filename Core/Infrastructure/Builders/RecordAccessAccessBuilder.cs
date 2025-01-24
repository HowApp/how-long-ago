namespace How.Core.Infrastructure.Builders;

using System.Text;
using HowCommon.Extensions;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.Record;
using Database.Entities.SharedUser;
using Enums;


public class RecordAccessAccessBuilder : IRecordAccessAccessBuilder
{
    private readonly StringBuilder _query = new StringBuilder();
    private readonly DynamicParameters _parameters = new DynamicParameters();

    public RecordAccessAccessBuilder(int eventId, int recordId)
    {
        _query.Append($@"SELECT 1 FROM {nameof(BaseDbContext.Events).ToSnake()} e WHERE 1 = 0");
        
        if (eventId >= 0 && recordId >= 0)
        {
            Init(eventId, recordId);
        }
    }

    public void Init(int eventId, int recordId)
    {
        _query.Clear();
        
        _query.Append($@"
SELECT 1 FROM {nameof(BaseDbContext.Events).ToSnake()} e
         RIGHT JOIN {nameof(BaseDbContext.Records).ToSnake()} r ON r.{nameof(Record.EventId).ToSnake()} = @eventId
    WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE AND
    e.{nameof(BaseCreated.Id).ToSnake()} = @eventId AND
    r.{nameof(BaseCreated.Id).ToSnake()} = @recordId
");
        _parameters.Add("@eventId", eventId);
        _parameters.Add("@recordId", recordId);
    }

    public void FilterCreatedBy(int userId, InternalAccessFilter internalAccessFilter)
    {
        _parameters.Add("@createdById", userId);
        
        switch (internalAccessFilter)
        {
            case InternalAccessFilter.IncludeCreatedBy:
                _query.Append($@"
    AND
    e.{nameof(BaseCreated.CreatedById).ToSnake()} = @created_by_id
");
                break;
            case InternalAccessFilter.IncludeShared:
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
                throw new ArgumentOutOfRangeException(nameof(internalAccessFilter), internalAccessFilter, null);
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