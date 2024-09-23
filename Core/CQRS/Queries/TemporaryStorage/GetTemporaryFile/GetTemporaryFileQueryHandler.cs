namespace How.Core.CQRS.Queries.TemporaryStorage.GetTemporaryFile;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.TemporaryStorageEntity;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;

public class GetTemporaryFileQueryHandler : IQueryHandler<GetTemporaryFileQuery, Result<TemporaryFileModel>>
{
    private readonly ILogger<GetTemporaryFileQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetTemporaryFileQueryHandler(ILogger<GetTemporaryFileQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<TemporaryFileModel>> Handle(GetTemporaryFileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    f.{nameof(File.Name).ToSnake()} AS {nameof(TemporaryFileModel.FileName)},
    f.{nameof(File.Content).ToSnake()} AS {nameof(TemporaryFileModel.Content)}
FROM {nameof(TemporaryStorageDbContext.Files).ToSnake()} f
WHERE f.{nameof(PKey.Id).ToSnake()} = @fileId
LIMIT 1;
";
            
            await using var connection = _dapper.InitTemporaryConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TemporaryFileModel>(
                query, 
                new
                {
                    fileId = request.FileId
                });

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<TemporaryFileModel>(
                new Error(ErrorType.TemporaryuFile, $"Error while executing {nameof(GetTemporaryFileQuery)}"));
        }
    }
}