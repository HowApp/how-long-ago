namespace How.Core.CQRS.Commands.TemporaryStorage.InsertTemporaryFile;

using Common.CQRS;
using Common.ResultType;
using Models.ServicesModel;

public sealed class InsertTemporaryFileCommand : ICommand<Result<int>>
{
    public TemporaryFileModel File { get; set; }
}