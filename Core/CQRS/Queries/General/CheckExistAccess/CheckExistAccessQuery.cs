namespace How.Core.CQRS.Queries.General.CheckExistAccess;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Builders;

public sealed class CheckExistAccessQuery : IQuery<Result<bool>>
{
    public IQueryAccessBuilder QueryAccessBuilder { get; set; }
}