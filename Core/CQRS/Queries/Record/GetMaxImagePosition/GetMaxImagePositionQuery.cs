namespace How.Core.CQRS.Queries.Record.GetMaxImagePosition;

using Common.CQRS;
using Common.ResultType;

public sealed class GetMaxImagePositionQuery : IQuery<Result<int>>
{
    public int RecordId { get; set; }
}