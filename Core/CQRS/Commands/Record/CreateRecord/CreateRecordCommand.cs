namespace How.Core.CQRS.Commands.Record.CreateRecord;

using Common.CQRS;
using Common.ResultType;

public sealed class CreateRecordCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
    public string Description { get; set; }
}