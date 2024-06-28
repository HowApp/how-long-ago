namespace How.Core.CQRS.Commands.Record.UpdateRecordImagePosition;

using System.Text;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class UpdateRecordImagePositionCommandHandler : ICommandHandler<UpdateRecordImagePositionCommand, Result<int>>
{
    private readonly ILogger<UpdateRecordImagePositionCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateRecordImagePositionCommandHandler(
        ILogger<UpdateRecordImagePositionCommandHandler> logger,
        DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(UpdateRecordImagePositionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ImageIds.Length == 0)
            {
                return Result.Failure<int>(new Error(ErrorType.Record, "Input array is empty"));
            }
            
            var command = new StringBuilder();
            var parameters = new DynamicParameters();

            for (int i = 0; i < request.ImageIds.Length; i++)
            {
                command.Append($@"
UPDATE {nameof(BaseDbContext.RecordImages).ToSnake()}
SET 
    {nameof(RecordImage.Position).ToSnake()} = @position_{i}
WHERE 
    {nameof(RecordImage.Id).ToSnake()} = @id_{i}
RETURNING *;
");
                parameters.AddDynamicParams( 
                    new Dictionary<string, object>
                    {
                        { $"@position_{i}", i},
                        { $"@id_{i}", request.ImageIds[i]}
                    });
            }
            
            await using var connection = _dapper.InitConnection();
            var result = await connection.ExecuteAsync(command.ToString(), parameters);

            return Result.Success<int>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Record, $"Error while executing {nameof(UpdateRecordImagePositionCommand)}"));
        }
    }
}