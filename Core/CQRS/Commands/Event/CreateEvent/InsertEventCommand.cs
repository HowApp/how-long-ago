namespace How.Core.CQRS.Commands.Event.CreateEvent;

using Common.CQRS;
using Common.ResultType;

public sealed class InsertEventCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public string Name { get; set; }
}