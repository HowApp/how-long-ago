namespace How.Core.CQRS.Commands.TemporaryStorage.DeleteTemporaryFile;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.TemporaryStorageEntity;
using Microsoft.Extensions.Logging;

public class DeleteTemporaryFileCommandHandler : ICommandHandler<DeleteTemporaryFileCommand, Result>
{
    private readonly ILogger<DeleteTemporaryFileCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteTemporaryFileCommandHandler(ILogger<DeleteTemporaryFileCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result> Handle(DeleteTemporaryFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var removeFileSql = $@"
DELETE FROM {nameof(TemporaryStorageDbContext.Files).ToSnake()}
WHERE {nameof(File.Id).ToSnake()} = ANY(@imageIds);
";
            await using var connection = _dapper.InitTemporaryConnection();
            var result = await connection.ExecuteAsync(removeFileSql, new
            {
                imageIds = request.FileId.ToArray()
            });
            
            if (result == 0)
            {
                _logger.LogError($"Error while insert {nameof(File)} at {nameof(DeleteTemporaryFileCommand)}");
                return Result.Failure<int>(new Error(ErrorType.TemporaryuFile, "Temporary File was not deleted!"));
            }
            
            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.TemporaryuFile, $"Error while executing {nameof(DeleteTemporaryFileCommand)}"));
        }
    }
}