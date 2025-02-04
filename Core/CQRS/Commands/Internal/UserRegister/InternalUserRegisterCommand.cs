namespace How.Core.CQRS.Commands.Internal.UserRegister;

using Common.CQRS;
using Common.ResultType;

public class InternalUserRegisterCommand : ICommand<Result<int>>
{
    public int UserId { get; set; }
}