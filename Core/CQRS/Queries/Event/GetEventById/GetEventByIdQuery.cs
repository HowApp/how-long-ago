namespace How.Core.CQRS.Queries.Event.GetEventById;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class GetEventByIdQuery : IQuery<Result<GetEventByIdQueryResult>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public InternalAccessFilter InternalAccessFilter { get; set; }
}