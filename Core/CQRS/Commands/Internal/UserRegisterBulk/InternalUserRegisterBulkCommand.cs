namespace How.Core.CQRS.Commands.Internal.UserRegisterBulk;

using Common.CQRS;
using Common.ResultType;

public class InternalUserRegisterBulkCommand : ICommand<Result<int>>
{
    public int[] UserIds { get; set; } = [];
}