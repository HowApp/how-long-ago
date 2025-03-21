namespace How.Core.CQRS.Commands.Event.UpdateEventImage;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;
using NodaTime;

public class UpdateEventImageCommandHandler : ICommandHandler<UpdateEventImageCommand, Result>
{
    private readonly ILogger<UpdateEventImageCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public UpdateEventImageCommandHandler(ILogger<UpdateEventImageCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result> Handle(UpdateEventImageCommand request, CancellationToken cancellationToken)
    {
        await using var connection = _dapper.InitConnection();
        await using var transaction = await connection.BeginTransactionAsync(CancellationToken.None);

        try
        {
            var updateImageSql = $@"
UPDATE {nameof(BaseDbContext.Events).ToSnake()}
SET 
{nameof(Event.StorageImageId).ToSnake()} = @imageId,
{nameof(Event.ChangedById).ToSnake()} = @userId,
{nameof(Event.ChangedAt).ToSnake()} = @changedAt
WHERE {nameof(Event.Id).ToSnake()} = @eventId
RETURNING (
    SELECT coalesce(u.{nameof(Event.StorageImageId).ToSnake()}, 0) 
    FROM {nameof(BaseDbContext.Events).ToSnake()} u 
    WHERE u.{nameof(Event.Id).ToSnake()} = @eventId);
";

            var oldImageId = await connection.QueryFirstOrDefaultAsync<int>(
                updateImageSql, new
                {
                    imageId = request.ImageId,
                    eventId = request.EventId,
                    userId = request.CurrentUserId,
                    changedAt = SystemClock.Instance.GetCurrentInstant()
                },
                transaction);

            if (oldImageId != 0)
            {
                var removeImageSql = $@"
DELETE FROM {nameof(BaseDbContext.StorageImages).ToSnake()}
WHERE {nameof(StorageImage.Id).ToSnake()} = @imageId
RETURNING {nameof(StorageImage.MainId).ToSnake()}, {nameof(StorageImage.ThumbnailId).ToSnake()};
";
                var oldFiles = await connection.QueryFirstOrDefaultAsync<(int,int)>(
                    removeImageSql, new
                    {
                        imageId = oldImageId
                    },
                    transaction);

                var removeFileSql = $@"
DELETE FROM {nameof(BaseDbContext.StorageFiles).ToSnake()}
WHERE {nameof(StorageFile.Id).ToSnake()} = ANY(@imageId);
";
                await connection.ExecuteAsync(removeFileSql, new
                    {
                        imageId = new int[] {oldFiles.Item1, oldFiles.Item2 }
                    },
                    transaction);
            }

            await transaction.CommitAsync(CancellationToken.None);
            return Result.Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(e.Message);
            return Result.Failure(
                new Error(ErrorType.Event, $"Error while executing {nameof(UpdateEventImageCommand)}"));
        }
    }
}