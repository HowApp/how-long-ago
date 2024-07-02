namespace How.Core.CQRS.Queries.Public.Event.GetEventsPagination;

using Models.Event;

public sealed class GetEventsPaginationQueryResult
{
    public int Count { get; set; }
    
    public ICollection<EventItemModel> Events { get; set; }
}