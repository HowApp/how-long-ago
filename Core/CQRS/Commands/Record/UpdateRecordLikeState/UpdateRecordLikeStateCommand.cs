namespace How.Core.CQRS.Commands.Record.UpdateRecordLikeState;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class UpdateRecordLikeStateCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int RecordId { get; set; }
    public LikeState LikeState { get; set; }
}