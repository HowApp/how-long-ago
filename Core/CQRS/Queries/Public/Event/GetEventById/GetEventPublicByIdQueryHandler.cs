namespace How.Core.CQRS.Queries.Public.Event.GetEventById;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.Identity;
using Database.Entities.Storage;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using Models.Event;

public class GetEventPublicByIdQueryHandler : IQueryHandler<GetEventPublicByIdQuery, Result<GetEventPublicByIdQueryResult>>
{
    private readonly ILogger<GetEventPublicByIdQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetEventPublicByIdQueryHandler(ILogger<GetEventPublicByIdQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetEventPublicByIdQueryResult>> Handle(GetEventPublicByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    e.{nameof(PKey.Id).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.Id)},
    e.{nameof(Event.Name).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.Name)},
    e.{nameof(Event.CreatedAt).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.CreatedAt)},
    event_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.EventMainHash)},
    event_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.EventThumbnailHash)},
    u.{nameof(PKey.Id).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.OwnerId)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.OwnerFirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.OwnerLastName)},
    user_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.OwnerMainHash)},
    user_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventPublicByIdQueryResult.OwnerThumbnailHash)},
    user_likes.likes AS {nameof(GetEventPublicByIdQueryResult.Likes)},
    user_likes.dislikes AS {nameof(GetEventPublicByIdQueryResult.Dislikes)},
    es.saved_count AS {nameof(GetEventPublicByIdQueryResult.SavedCount)}
FROM {nameof(BaseDbContext.Events).ToSnake()} e
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} event_image ON 
    e.{nameof(Event.StorageImageId).ToSnake()} = event_image.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_main ON 
    event_image.{nameof(StorageImage.MainId).ToSnake()} = event_main.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_thumbnail ON 
    event_image.{nameof(StorageImage.ThumbnailId).ToSnake()} = event_thumbnail.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.Users).ToSnake()} u ON 
    e.{nameof(Event.OwnerId).ToSnake()} = u.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} user_image ON 
    u.{nameof(HowUser.StorageImageId).ToSnake()} = user_image.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} user_main ON 
    user_image.{nameof(StorageImage.MainId).ToSnake()} = user_main.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} user_thumbnail ON 
    user_image.{nameof(StorageImage.ThumbnailId).ToSnake()} = user_thumbnail.{nameof(PKey.Id).ToSnake()}
LEFT JOIN (
        SELECT 
            le.{nameof(LikedEvent.EventId).ToSnake()} AS liked_event_id,
            COUNT(CASE WHEN le.{nameof(LikedEvent.State).ToSnake()} = 2 THEN 1 END) AS likes,
            COUNT(CASE WHEN le.{nameof(LikedEvent.State).ToSnake()} = 3 THEN 1 END) AS dislikes
        FROM {nameof(BaseDbContext.LikedEvents).ToSnake()} le 
        GROUP BY le.{nameof(LikedEvent.EventId).ToSnake()}
        ) user_likes ON e.{nameof(PKey.Id).ToSnake()} = user_likes.liked_event_id
LEFT JOIN (
    SELECT 
        se_count.{nameof(SavedEvent.EventId).ToSnake()} AS saved_count_id,
        COUNT(se_count.{nameof(SavedEvent.UserId).ToSnake()}) AS saved_count
    FROM {nameof(BaseDbContext.SavedEvents).ToSnake()} se_count
    GROUP BY {nameof(SavedEvent.EventId).ToSnake()}
) es ON es.saved_count_id = e.{nameof(PKey.Id).ToSnake()}
WHERE e.{nameof(PKey.Id).ToSnake()} = @EventId
    AND
    e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
    e.{nameof(Event.Status).ToSnake()} = @status
    AND
    e.{nameof(Event.Access).ToSnake()} = @access
LIMIT 1;
";
            
            await using var connection = _dapper.InitConnection();
            
            var eventItem = await connection.QueryFirstOrDefaultAsync<GetEventPublicByIdQueryResult>(
                query,
                new
                {
                    status = (int)EventStatus.Active,
                    access = (int)EventAccessType.Public,
                    EventId = request.EventId,
                });
            
            return Result.Success(eventItem);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventPublicByIdQueryResult>(
                new Error(ErrorType.Event, $"Error while executing {nameof(GetEventPublicByIdQuery)}"));
        }
    }
}