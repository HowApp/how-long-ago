namespace How.Core.CQRS.Commands.Record.UpdateRecord;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class UpdateRecordCommandHandler : ICommandHandler<UpdateRecordCommand, Result<int>>
{
    private readonly ILogger<UpdateRecordCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateRecordCommandHandler(ILogger<UpdateRecordCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
UPDATE {nameof(BaseDbContext.Records).ToSnake()}
SET
    {nameof(Record.Description).ToSnake()} = @description
WHERE 
    {nameof(Record.Id).ToSnake()} = @record_id
RETURNING *;
";

            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command,
                new
                {
                    description = request.Description,
                    record_id = request.RecordId
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error while executing {nameof(UpdateRecordCommand)}"));
        }
    }
}