namespace How.Core.CQRS.Commands.Internal.UserUpdateSuspend;

using Common.CQRS;
using Common.ResultType;

public class InternalUserUpdateSuspendCommand : ICommand<Result<int>>
{
    public int UserId { get; set; }
    public bool State { get; set; }
}