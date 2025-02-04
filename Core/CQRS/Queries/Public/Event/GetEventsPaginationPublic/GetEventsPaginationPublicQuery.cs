namespace How.Core.CQRS.Queries.Public.Event.GetEventsPaginationPublic;

using Common.CQRS;
using Common.Models;
using Common.ResultType;

public sealed class GetEventsPaginationPublicQuery : PaginationModel, IQuery<Result<GetEventsPaginationPublicQueryResult>>
{
    public string Search { get; set; }
}