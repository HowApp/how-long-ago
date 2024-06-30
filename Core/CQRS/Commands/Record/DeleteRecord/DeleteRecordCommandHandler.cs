namespace How.Core.CQRS.Commands.Record.DeleteRecord;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Microsoft.Extensions.Logging;

public class DeleteRecordCommandHandler : ICommandHandler<DeleteRecordCommand, Result<int>>
{
    private readonly ILogger<DeleteRecordCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteRecordCommandHandler(ILogger<DeleteRecordCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(DeleteRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = $@"
DELETE FROM records
WHERE id = ANY(@recordIds)
RETURNING *;
";
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(
                command, new
                {
                    recordIds = request.RecordIds
                });
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(DeleteRecordCommand)}"));
        }
    }
}