namespace How.Core.CQRS.Commands.Event.UpdateEvent;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateEventCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; }
}