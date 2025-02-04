namespace How.Core.Infrastructure.Builders;

using Enums;

public interface IRecordAccessAccessBuilder : IQueryAccessBuilder
{
    void Init(int eventId, int recordId);
    void FilterCreatedBy(int userId, InternalAccessFilter internalAccessFilter);
    void FilterByStatus(EventStatus status);
    void FilterByAccessType(EventAccessType accessType);
}