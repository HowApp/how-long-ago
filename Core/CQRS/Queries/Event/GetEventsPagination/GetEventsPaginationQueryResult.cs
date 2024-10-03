namespace How.Core.CQRS.Queries.Event.GetEventsPagination;

using Models.Event;

public sealed class GetEventsPaginationQueryResult
{
    public int Count { get; set; }
    
    public ICollection<EventItemPrivateModel> Events { get; set; }
}