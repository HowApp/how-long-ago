namespace How.Core.CQRS.Queries.Event.CheckEvent;

using Common.CQRS;
using Common.ResultType;
using Infrastructure.Builders;

public sealed class CheckEventQuery : IQuery<Result<bool>>
{
    public IEventAccessQueryBuilder QueryBuilder { get; set; } = new EventAccessQueryBuilder();
}