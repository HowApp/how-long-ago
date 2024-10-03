namespace How.Core.Infrastructure.Builders;

using Enums;

public interface IEventAccessQueryAccessBuilder : IQueryAccessBuilder
{
    void Init(int eventId);
    void FilterCreatedBy(int userId, InternalAccessFilter internalAccessFilter);
    void FilterByStatus(EventStatus status);
    void FilterByAccessType(EventAccessType accessType);
    
}