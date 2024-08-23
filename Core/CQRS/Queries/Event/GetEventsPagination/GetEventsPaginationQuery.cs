namespace How.Core.CQRS.Queries.Event.GetEventsPagination;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class GetEventsPaginationQuery : PaginationModel, IQuery<Result<GetEventsPaginationQueryResult>>
{
    public int CurrentUserId { get; set; }
    public string Search { get; set; }
    public EventStatus Status { get; set; }
    public EventAccessType Access { get; set; }
    public AccessFilterType AccessFilterType { get; set; }
}