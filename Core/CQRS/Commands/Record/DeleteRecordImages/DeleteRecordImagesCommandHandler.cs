namespace How.Core.CQRS.Commands.Record.DeleteRecordImages;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Record;
using Microsoft.Extensions.Logging;

public class DeleteRecordImagesCommandHandler : ICommandHandler<DeleteRecordImagesCommand, Result<int[]>>
{
    private readonly ILogger<DeleteRecordImagesCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public DeleteRecordImagesCommandHandler(ILogger<DeleteRecordImagesCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int[]>> Handle(DeleteRecordImagesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var removeRecordImagesSql = $@"
DELETE FROM {nameof(BaseDbContext.RecordImages).ToSnake()}
WHERE {nameof(PKey.Id).ToSnake()} = ANY(@imageId)
RETURNING {nameof(RecordImage.ImageId).ToSnake()};
";

            await using var connection = _dapper.InitConnection();
            var storageImageIds = await connection.QueryAsync<int>(
                removeRecordImagesSql, new
                {
                    imageId = request.ImageIds
                });

            return Result.Success(storageImageIds.ToArray());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<int[]>(
                new Error(ErrorType.Storage, $"Error while executing {nameof(DeleteRecordImagesCommand)}"));
        }
    }
}