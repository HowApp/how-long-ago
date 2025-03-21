namespace How.Core.Infrastructure.Builders;

using Enums;

public interface IEventAccessQueryAccessBuilder : IQueryAccessBuilder
{
    void Init(int eventId);
    void FilterByInternalAccessFilter(int userId, InternalAccessFilter internalAccessFilter);
    void FilterByStatus(EventStatus status);
    void FilterByAccessType(EventAccessType accessType);
}