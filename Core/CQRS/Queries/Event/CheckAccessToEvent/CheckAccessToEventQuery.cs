namespace How.Core.CQRS.Queries.Event.CheckAccessToEvent;

using Common.CQRS;
using Common.ResultType;

public sealed class CheckAccessToEventQuery : IQuery<Result<bool>>
{
    public int EventId { get; set; }
    public int CurrentUserId { get; set; }
}