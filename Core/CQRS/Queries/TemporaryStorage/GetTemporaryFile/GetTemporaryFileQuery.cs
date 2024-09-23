namespace How.Core.CQRS.Queries.TemporaryStorage.GetTemporaryFile;

using Common.CQRS;
using Common.ResultType;
using Models.ServicesModel;

public sealed class GetTemporaryFileQuery : IQuery<Result<TemporaryFileModel>>
{
    public int FileId { get; set; }
}