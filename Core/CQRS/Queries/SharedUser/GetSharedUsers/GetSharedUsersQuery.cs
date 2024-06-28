namespace How.Core.CQRS.Queries.SharedUser.GetSharedUsers;

using Common.CQRS;
using Common.ResultType;

public sealed class GetSharedUsersQuery : IQuery<Result<List<GetSharedUsersQueryResult>>>
{
    public int CurrentUserId { get; set; }
}