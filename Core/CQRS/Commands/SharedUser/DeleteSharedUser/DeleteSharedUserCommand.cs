namespace How.Core.CQRS.Commands.SharedUser.DeleteSharedUser;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteSharedUserCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public int SharedUserId { get; set; }
}