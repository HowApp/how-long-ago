namespace How.Core.CQRS.Commands.Storage.DeleteImageMultiply;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteImageMultiplyCommand : ICommand<Result>
{
    public int[] ImageIds { get; set; }
}