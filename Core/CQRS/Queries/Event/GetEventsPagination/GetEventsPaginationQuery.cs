namespace How.Core.CQRS.Queries.Event.GetEventsPagination;

using Common.CQRS;
using Common.Models;
using Common.ResultType;

public sealed class GetEventsPaginationQuery : PaginationModel, IQuery<Result<GetEventsPaginationQueryResult>>
{
    public string Search { get; set; }
}