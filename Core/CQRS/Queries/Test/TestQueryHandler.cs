namespace How.Core.CQRS.Queries.Test;

using Common.CQRS;
using Common.ResultType;
using Microsoft.Extensions.Logging;
using Shared.DTO.Test;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, Result<TestPostResponseDTO>>
{
    private readonly ILogger<TestQueryHandler> _logger;

    public TestQueryHandler(ILogger<TestQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TestPostResponseDTO>> Handle(TestQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = new TestPostResponseDTO
            {
                MessageFromQuery = $"Your Number is {request.Number}"
            };

            return Result.Success<TestPostResponseDTO>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<TestPostResponseDTO>(new Error(
                ErrorType.Account,
                $"Error while executing {nameof(TestQueryHandler)}!"));
        }
    }
}