namespace How.Core.CQRS.Commands.Internal.UserDelete;

using Common.CQRS;
using Common.ResultType;

public class InternalUserDeleteCommand : ICommand<Result<int>>
{
    public int UserId { get; set; }
    public string Salt { get; set; }
}