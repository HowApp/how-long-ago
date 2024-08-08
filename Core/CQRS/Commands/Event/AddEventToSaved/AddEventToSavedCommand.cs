namespace How.Core.CQRS.Commands.Event.AddEventToSaved;

using Common.CQRS;
using Common.ResultType;

public sealed class AddEventToSavedCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
}