namespace How.Core.CQRS.Commands.Record.UpdateRecordImagePosition;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateRecordImagePositionCommand : ICommand<Result<int>>
{
    public int[] ImageIds { get; set; } = [];
}