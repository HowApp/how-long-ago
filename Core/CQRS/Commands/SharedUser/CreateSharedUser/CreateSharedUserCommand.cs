namespace How.Core.CQRS.Commands.SharedUser.CreateSharedUser;

using Common.CQRS;
using Common.ResultType;

public sealed class CreateSharedUserCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int SharedUserId { get; set; }
}