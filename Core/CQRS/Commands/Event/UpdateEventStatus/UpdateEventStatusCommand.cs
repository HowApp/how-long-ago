namespace How.Core.CQRS.Commands.Event.UpdateEventStatus;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class UpdateEventStatusCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public EventStatus Status { get; set; }
}