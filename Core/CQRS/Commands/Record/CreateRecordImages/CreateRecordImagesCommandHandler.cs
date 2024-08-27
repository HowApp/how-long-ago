namespace How.Core.CQRS.Commands.Record.CreateRecordImages;

using System.Text;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class CreateRecordImagesCommandHandler : ICommandHandler<CreateRecordImagesCommand, Result<int[]>>
{
    private readonly ILogger<CreateRecordImagesCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public CreateRecordImagesCommandHandler(ILogger<CreateRecordImagesCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int[]>> Handle(CreateRecordImagesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var sql = new StringBuilder();
            var replacedItem = "@record_id, @position, @image_id";
            
            var command = $@"
INSERT INTO {nameof(BaseDbContext.RecordImages).ToSnake()} (
    {nameof(RecordImage.RecordId).ToSnake()},
    {nameof(RecordImage.Position).ToSnake()},
    {nameof(RecordImage.ImageId).ToSnake()}
)
VALUES ({replacedItem})
RETURNING {nameof(PKey.Id).ToSnake()};
";
            sql.Append(command);
        
            var values = new List<string>();
            var parameters = new DynamicParameters();
            
            for (int i = 0; i < request.ImageIds.Length; i++)
            {
                values.Add(@$"(@record_id_{i}, @position_{i}, @image_id_{i})");
            
                parameters.AddDynamicParams(
                    new Dictionary<string, object>
                    {
                        { $"@record_id_{i}", request.RecordId},
                        { $"@position_{i}", i + request.MaxPosition},
                        { $"@image_id_{i}", request.ImageIds[i]}
                    });
            }

            sql.Replace($"({replacedItem})", string.Join(", \n", values));
        
            await using var connection = _dapper.InitConnection();
            var result = await connection.QueryAsync<int>(sql.ToString(), parameters);

            return Result.Success<int[]>(result.ToArray());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int[]>(
                new Error(ErrorType.Event, $"Error while executing {nameof(CreateRecordImagesCommand)}"));
        }
    }
}