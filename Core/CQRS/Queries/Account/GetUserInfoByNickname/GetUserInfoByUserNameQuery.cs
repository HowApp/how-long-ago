namespace How.Core.CQRS.Queries.Account.GetUserInfoByNickname;

using Common.CQRS;
using Common.ResultType;

public sealed class GetUserInfoByUserNameQuery : IQuery<Result<List<GetUserInfoByUserNameQueryResult>>>
{
    public string Search { get; set; }
}