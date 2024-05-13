namespace How.Core.CQRS.Commands.Account.UpdateUserImage;

using Common.CQRS;
using Common.ResultType;

public sealed class UpdateUserImageCommand : ICommand<Result>
{
    public int CurrentUserId { get; set; }
    public int ImageId { get; set; }
}