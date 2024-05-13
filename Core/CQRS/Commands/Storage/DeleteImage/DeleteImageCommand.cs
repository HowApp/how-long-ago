namespace How.Core.CQRS.Commands.Storage.DeleteImage;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteImageCommand : ICommand<Result>
{
    public int ImageId { get; set; }
}