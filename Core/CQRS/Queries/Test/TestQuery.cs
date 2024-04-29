namespace How.Core.CQRS.Queries.Test;

using Common.CQRS;
using Common.ResultType;
using DTO.Test;

public sealed record TestQuery : IQuery<Result<TestPostResponseDTO>>
{
    public int Number { get; set; }
}