namespace How.Core.CQRS.Commands.TemporaryStorage.InsertTemporaryFile;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.TemporaryStorageEntity;
using Microsoft.Extensions.Logging;

public class InsertTemporaryFileCommandHandler : ICommandHandler<InsertTemporaryFileCommand, Result<int>>
{
    private readonly ILogger<InsertTemporaryFileCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InsertTemporaryFileCommandHandler(ILogger<InsertTemporaryFileCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(InsertTemporaryFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = _dapper.InitTemporaryConnection();
            
            var command = $@"
INSERT INTO {nameof(TemporaryStorageDbContext.Files).ToSnake()} (
    {nameof(File.Name).ToSnake()},
    {nameof(File.Content).ToSnake()}) 
VALUES 
    (@name, @content)
RETURNING {nameof(File.Id).ToSnake()};
";
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                command,
                new
                {
                    name = request.File.FileName,
                    content = request.File.Content
                });
            
            if (result == 0)
            {
                _logger.LogError($"Error while insert {nameof(File)} at {nameof(InsertTemporaryFileCommand)}");
                return Result.Failure<int>(new Error(ErrorType.TemporaryuFile, "Temporary File was not inserted!"));
            }
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.TemporaryuFile, $"Error while executing {nameof(InsertTemporaryFileCommandHandler)}"));
        }
    }
}