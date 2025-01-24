namespace How.Core.CQRS.Commands.Record.UpdateRecordLikeState;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class UpdateRecordLikeStateCommandHandler : ICommandHandler<UpdateRecordLikeStateCommand, Result<int>>
{
    private readonly ILogger<UpdateRecordLikeStateCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateRecordLikeStateCommandHandler(ILogger<UpdateRecordLikeStateCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateRecordLikeStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
INSERT INTO {nameof(BaseDbContext.LikedRecords).ToSnake()} (
   {nameof(LikedRecord.RecordId).ToSnake()},
   {nameof(LikedRecord.LikedByUserId).ToSnake()},
   {nameof(LikedRecord.State).ToSnake()}
)
VALUES (@recordId, @likedByUserId, @state)
ON CONFLICT ({nameof(LikedRecord.RecordId).ToSnake()}, {nameof(LikedRecord.LikedByUserId).ToSnake()})
DO UPDATE SET 
    {nameof(LikedRecord.State).ToSnake()} = @state;

SELECT *
FROM {nameof(BaseDbContext.LikedRecords).ToSnake()} lr 
WHERE 
    lr.{nameof(LikedRecord.LikedByUserId).ToSnake()} = @likedByUserId 
  AND 
    lr.{nameof(LikedRecord.RecordId).ToSnake()} = @recordId;
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.QuerySingleAsync<int>(
                command,
                new
                {
                    recordId = request.RecordId,
                    likedByUserId = request.CurrentUserId,
                    state = request.LikeState
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error while executing {nameof(UpdateRecordLikeStateCommand)}"));
        }
    }
}