namespace How.Core.CQRS.Queries.Event.GetEventById;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.Identity;
using Database.Entities.Storage;
using Database.Entities.SharedUser;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;

public class GetEventByIdQueryHandler : IQueryHandler<GetEventByIdQuery, Result<GetEventByIdQueryResult>>
{
    private readonly ILogger<GetEventByIdQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetEventByIdQueryHandler(ILogger<GetEventByIdQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetEventByIdQueryResult>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var innerFilter = string.Empty;

            switch (request.InternalAccessFilter)
            {
                case InternalAccessFilter.IncludeCreatedBy:
                    innerFilter = $@"e.{nameof(Event.OwnerId).ToSnake()} = @created_by_id";
                    break;
                case InternalAccessFilter.IncludeShared:
                    innerFilter = $@"
e.{nameof(Event.OwnerId).ToSnake()} = @created_by_id
OR
EXISTS(
    SELECT 1 
    FROM {nameof(BaseDbContext.SharedUsers).ToSnake()} su 
    WHERE 
        su.{nameof(SharedUser.UserOwnerId).ToSnake()} = e.{nameof(Event.OwnerId).ToSnake()}
      AND 
        su.{nameof(SharedUser.UserSharedId).ToSnake()} = @created_by_id)";
                    break;
                default:
                    innerFilter = $@"true";
                    break;
            }
            
            var innerStatusFilter = string.Empty;
            switch (request.Status)
            {
                case EventStatusFilter.None:
                    innerStatusFilter = $@"true";
                    break;
                default:
                    innerStatusFilter = $@"e.{nameof(Event.Status).ToSnake()} = {(int)request.Status}";
                    break;
            }
            
            var innerAccessFilter = string.Empty;
            switch (request.Access)
            {
                case EventAccessFilter.None:
                    innerAccessFilter = $@"true";
                    break;
                default:
                    innerAccessFilter = $@"e.{nameof(Event.Access).ToSnake()} = {(int)request.Access}";
                    break;
            }
            
            var query = $@"
SELECT 
    e.{nameof(PKey.Id).ToSnake()} AS {nameof(GetEventByIdQueryResult.Id)},
    e.{nameof(Event.Name).ToSnake()} AS {nameof(GetEventByIdQueryResult.Name)},
    e.{nameof(Event.CreatedAt).ToSnake()} AS {nameof(GetEventByIdQueryResult.CreatedAt)},
    e.{nameof(Event.Status).ToSnake()} As {nameof(GetEventByIdQueryResult.Status)},
    e.{nameof(Event.Access).ToSnake()} As {nameof(GetEventByIdQueryResult.Access)},
    event_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventByIdQueryResult.EventMainHash)},
    event_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventByIdQueryResult.EventThumbnailHash)},
    u.{nameof(HowUser.UserId).ToSnake()} AS {nameof(GetEventByIdQueryResult.OwnerId)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(GetEventByIdQueryResult.OwnerFirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(GetEventByIdQueryResult.OwnerLastName)},
    user_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventByIdQueryResult.OwnerMainHash)},
    user_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(GetEventByIdQueryResult.OwnerThumbnailHash)},
    user_likes.likes AS {nameof(GetEventByIdQueryResult.Likes)},
    user_likes.dislikes AS {nameof(GetEventByIdQueryResult.Dislikes)},
    user_likes.current_user_state AS {nameof(GetEventByIdQueryResult.OwnLikeState)},
    (SELECT EXISTS(
        SELECT 1 FROM {nameof(BaseDbContext.SavedEvents).ToSnake()} se 
                 WHERE se.{nameof(SavedEvent.EventId).ToSnake()} = e.{nameof(PKey.Id).ToSnake()} AND
                       se.{nameof(SavedEvent.UserId).ToSnake()} = @created_by_id)
     ) AS {nameof(GetEventByIdQueryResult.IsSavedByUser)},
    es.saved_count AS {nameof(GetEventByIdQueryResult.SavedCount)}
FROM {nameof(BaseDbContext.Events).ToSnake()} e
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} event_image ON 
    e.{nameof(Event.StorageImageId).ToSnake()} = event_image.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_main ON 
    event_image.{nameof(StorageImage.MainId).ToSnake()} = event_main.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_thumbnail ON 
    event_image.{nameof(StorageImage.ThumbnailId).ToSnake()} = event_thumbnail.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.Users).ToSnake()} u ON 
    e.{nameof(Event.OwnerId).ToSnake()} = u.{nameof(HowUser.UserId).ToSnake()}
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
            COUNT(CASE WHEN le.{nameof(LikedEvent.State).ToSnake()} = 3 THEN 1 END) AS dislikes,
            coalesce(le_u.{nameof(LikedEvent.State).ToSnake()}, 1) AS current_user_state
        FROM {nameof(BaseDbContext.LikedEvents).ToSnake()} le 
        LEFT JOIN {nameof(BaseDbContext.LikedEvents).ToSnake()} le_u ON 
            le_u.{nameof(LikedEvent.EventId).ToSnake()} = le.{nameof(LikedEvent.EventId).ToSnake()} AND 
            le_u.{nameof(LikedEvent.LikedByUserId).ToSnake()} = @created_by_id
        GROUP BY 
            le.{nameof(LikedEvent.EventId).ToSnake()},
            le_u.{nameof(LikedEvent.State).ToSnake()}
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
    ({innerFilter})
    AND
    ({innerStatusFilter})
    AND
    ({innerAccessFilter})
LIMIT 1;
";
            
            await using var connection = _dapper.InitConnection();
            
            var eventItem = await connection.QueryFirstOrDefaultAsync<GetEventByIdQueryResult>(
                query,
                new
                {
                    created_by_id = request.CurrentUserId,
                    EventId = request.EventId
                });
            
            return Result.Success(eventItem);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventByIdQueryResult>(
                new Error(ErrorType.Event, $"Error while executing {nameof(GetEventByIdQuery)}"));
        }
    }
}