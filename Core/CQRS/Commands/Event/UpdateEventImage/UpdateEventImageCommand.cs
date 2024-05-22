namespace How.Core.CQRS.Commands.Event.UpdateEventImage;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateEventImageCommand : ICommand<Result>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public int ImageId { get; set; }
}