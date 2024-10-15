namespace How.Core.CQRS.Queries.Public.Record.GetRecordsPaginationPublic;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using DTO.Public.Record;

public sealed class GetRecordsPaginationPublicQuery : PaginationModel, IQuery<Result<GetRecordsPaginationPublicResponseDTO>>
{
    public int EventId { get; set; }
}