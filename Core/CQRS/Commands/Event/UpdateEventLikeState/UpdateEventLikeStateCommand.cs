namespace How.Core.CQRS.Commands.Event.UpdateEventLikeState;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class UpdateEventLikeStateCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public LikeState LikeState { get; set; }
}