namespace How.Core.CQRS.Queries.Public.Event.GetEventsPaginationPublic;

using Models.Event;

public sealed class GetEventsPaginationPublicQueryResult
{
    public int Count { get; set; }
    
    public ICollection<EventItemPublicModel> Events { get; set; }
}