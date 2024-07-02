namespace How.Core.CQRS.Queries.Public.Event.GetEventsPagination;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class GetEventsPaginationQuery : PaginationModel, IQuery<Result<GetEventsPaginationQueryResult>>
{
    public string Search { get; set; }
}