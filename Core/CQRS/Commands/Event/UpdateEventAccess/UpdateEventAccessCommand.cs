namespace How.Core.CQRS.Commands.Event.UpdateEventAccess;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class UpdateEventAccessCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public EventAccessType Access { get; set; }
}