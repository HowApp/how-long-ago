namespace How.Core.CQRS.Queries.Public.Event.GetEventById;

using Common.CQRS;
using Common.ResultType;

public sealed class GetEventByIdQuery : IQuery<Result<GetEventByIdQueryResult>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
}