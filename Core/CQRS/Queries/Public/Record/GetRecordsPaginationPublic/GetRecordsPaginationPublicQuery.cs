namespace How.Core.CQRS.Queries.Public.Record.GetRecordsPaginationPublic;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using DTO.Record;

public sealed class GetRecordsPaginationPublicQuery : PaginationModel, IQuery<Result<GetRecordsPaginationResponseDTO>>
{
    public int CurrentUserId { get; set; }
    public int EventId { get; set; }
}