namespace How.Core.CQRS.Queries.Record.GetImageIds;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class GetImageIdsQueryQueryHandler : IQueryHandler<GetImageIdsQuery, Result<int[]>>
{
    private readonly ILogger<GetImageIdsQueryQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetImageIdsQueryQueryHandler(ILogger<GetImageIdsQueryQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int[]>> Handle(GetImageIdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT ri.{nameof(RecordImage.Id).ToSnake()}
FROM {nameof(BaseDbContext.RecordImages).ToSnake()} ri
WHERE ri.{nameof(RecordImage.RecordId).ToSnake()} = @record_id
";
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.QueryAsync<int>(
                query,
                new
                {
                    record_id = request.RecordId
                });

            return Result.Success<int[]>(result.ToArray());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int[]>(
                new Error(ErrorType.Event, $"Error while executing {nameof(GetImageIdsQuery)}"));
        }
    }
}