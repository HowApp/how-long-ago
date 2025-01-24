namespace How.Core.CQRS.Queries.Record.GetMaxImagePosition;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class GetMaxImagePositionQueryHandler : IQueryHandler<GetMaxImagePositionQuery, Result<int>>
{
    private readonly ILogger<GetMaxImagePositionQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetMaxImagePositionQueryHandler(ILogger<GetMaxImagePositionQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(GetMaxImagePositionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT COALESCE(MAX(ri.{nameof(RecordImage.Position).ToSnake()}), -1)
FROM {nameof(BaseDbContext.RecordImages).ToSnake()} ri
WHERE ri.{nameof(RecordImage.RecordId).ToSnake()} = @record_id
";
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                query,
                new
                {
                    record_id = request.RecordId
                });

            return Result.Success<int>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error while executing {nameof(GetMaxImagePositionQuery)}"));
        }
    }
}