namespace How.Core.CQRS.Commands.Record.UpdateRecord;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateRecordCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public int RecordId { get; set; }
    public string Description { get; set; }
}