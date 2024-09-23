namespace How.Core.CQRS.Commands.TemporaryStorage.DeleteTemporaryFile;

using Common.CQRS;
using Common.ResultType;

public sealed class DeleteTemporaryFileCommand : ICommand<Result>
{
    public int[] FileId { get; set; }
}