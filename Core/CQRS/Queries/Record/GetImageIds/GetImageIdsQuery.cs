namespace How.Core.CQRS.Queries.Record.GetImageIds;

using Common.CQRS;
using Common.ResultType;

public sealed class GetImageIdsQuery : IQuery<Result<int[]>>
{
    public int RecordId { get; set; }
}