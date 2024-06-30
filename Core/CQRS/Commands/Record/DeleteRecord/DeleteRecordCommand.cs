namespace How.Core.CQRS.Commands.Record.DeleteRecord;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteRecordCommand : ICommand<Result<int>>
{
    public int[] RecordIds { get; set; }
}