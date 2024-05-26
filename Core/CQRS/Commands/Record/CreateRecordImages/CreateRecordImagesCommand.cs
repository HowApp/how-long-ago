namespace How.Core.CQRS.Commands.Record.CreateRecordImages;

using Common.CQRS;
using Common.ResultType;

public sealed class CreateRecordImagesCommand : ICommand<Result<int[]>>
{
    public int RecordId { get; set; }
    public int[] ImageIds { get; set; } = [];
    public int MaxPosition { get; set; }
}