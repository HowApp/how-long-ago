namespace How.Core.CQRS.Commands.Record.DeleteRecordImages;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteRecordImagesCommand : ICommand<Result<int[]>>
{
    public int[] ImageIds { get; set; } = [];
}