namespace How.Core.CQRS.Commands.Account.UpdateUserInfo;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateUserInfoCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}