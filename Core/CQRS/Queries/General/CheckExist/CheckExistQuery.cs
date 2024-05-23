namespace How.Core.CQRS.Queries.General.CheckExist;

using Common.CQRS;
using Common.ResultType;

public sealed class CheckExistQuery : IQuery<Result<bool>>
{
    public int Id { get; set; }
    public string Table { get; set; }
}