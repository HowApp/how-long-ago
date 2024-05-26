namespace How.Core.CQRS.Queries.Record.CheckRecordExist;

using Dapper;
using Database;
using Database.Entities.Base;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Database.Entities.Event;
using Microsoft.Extensions.Logging;

public class CheckRecordExistQueryHandler : IQueryHandler<CheckRecordExistQuery, Result<bool>>
{
    private readonly ILogger<CheckRecordExistQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public CheckRecordExistQueryHandler(ILogger<CheckRecordExistQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<bool>> Handle(CheckRecordExistQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT EXISTS(
    SELECT 1 FROM {nameof(BaseDbContext.Records).ToSnake()} 
    WHERE 
    {nameof(BaseCreeated.Id).ToSnake()} = @id AND
    {nameof(BaseCreeated.CreatedById).ToSnake()} = @created_by_id AND 
    {nameof(Record.EventId).ToSnake()} = @eventId);
";
            await using var connection = _dapper.InitConnection();
            
            var result = await connection.QuerySingleAsync<bool>(
                query,
                new
                {
                    created_by_id = request.CurrentUserId,
                    id = request.Id,
                    eventId = request.EventId
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<bool>(
                new Error(ErrorType.Record, $"Error while executing {nameof(CheckRecordExistQuery)}"));
        }
    }
}