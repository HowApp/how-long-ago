namespace How.Core.CQRS.Queries.Record.GetRecordsPagination;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using DTO.Record;

public sealed class GetRecordsPaginationQuery : PaginationModel, IQuery<Result<GetRecordsPaginationResponseDTO>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
}