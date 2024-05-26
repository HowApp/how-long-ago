namespace How.Core.CQRS.Queries.Record.CheckRecordExist;

using Common.CQRS;
using Common.ResultType;

public sealed class CheckRecordExistQuery : IQuery<Result<bool>>
{
    public int Id { get; set; }
    public int CurrentUserId { get; set; }
    
    public int EventId { get; set; }
}