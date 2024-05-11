namespace How.Core.CQRS.Queries.Account.GetUserInfo;

using Common.CQRS;
using Common.ResultType;

public sealed class GetUserInfoQuery : IQuery<Result<GetUserInfoQueryResult?>>
{
    public int CurrentUserId { get; set; }
}