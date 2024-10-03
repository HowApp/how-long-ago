namespace How.Core.CQRS.Queries.Event.GetEventsPagination;

using Common.CQRS;
using Common.Models;
using Common.ResultType;
using Infrastructure.Enums;

public sealed class GetEventsPaginationQuery : PaginationModel, IQuery<Result<GetEventsPaginationQueryResult>>
{
    public int CurrentUserId { get; set; }
    public string Search { get; set; }
    public EventStatusFilter Status { get; set; }
    public EventAccessFilter Access { get; set; }
    public InternalAccessFilter InternalAccessFilter { get; set; }
}