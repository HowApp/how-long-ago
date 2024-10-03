namespace How.Core.CQRS.Queries.Event.GetEventsPagination;

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
using Models.Event;

public class GetEventsPaginationQueryHandler : IQueryHandler<GetEventsPaginationQuery, Result<GetEventsPaginationQueryResult>>
{
    private readonly ILogger<GetEventsPaginationQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetEventsPaginationQueryHandler(ILogger<GetEventsPaginationQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetEventsPaginationQueryResult>> Handle(
        GetEventsPaginationQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var innerFilter = string.Empty;

            switch (request.AccessFilterType)
            {
                case AccessFilterType.None:
                    innerFilter = $@"
true";
                    break;
                case AccessFilterType.IncludeCreatedBy:
                    innerFilter = $@"
e.{nameof(Event.OwnerId).ToSnake()} = @created_by_id";
                    break;
                case AccessFilterType.IncludeShared:
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
                innerFilter = $@"
true";
                    break;
            }
            
            var accessFilter = string.Empty;
            switch (request.Access)
            {
                case EventAccessType.None:
                    accessFilter = $@"
true";
                    break;
                default:
                    accessFilter = $@"
e.{nameof(Event.Access).ToSnake()} = @access";
                    break;
            }
            
            var activeStatusFilter = string.Empty;
            switch (request.Status)
            {
                case EventStatus.None:
                    activeStatusFilter = $@"
true";
                    break;
                default:
                    activeStatusFilter = $@"
e.{nameof(Event.Status).ToSnake()} = @status";
                    break;
            }

            var query = $@"
SELECT 
    e.{nameof(PKey.Id).ToSnake()} AS {nameof(EventItemPrivateModel.Id)},
    e.{nameof(Event.Name).ToSnake()} AS {nameof(EventItemPrivateModel.Name)},
    e.{nameof(Event.CreatedAt).ToSnake()} AS {nameof(EventItemPrivateModel.CreatedAt)},
    e.{nameof(Event.Status).ToSnake()} As {nameof(EventItemPrivateModel.Status)},
    e.{nameof(Event.Access).ToSnake()} As {nameof(EventItemPrivateModel.Access)},
    event_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemPrivateModel.EventMainHash)},
    event_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemPrivateModel.EventThumbnailHash)},
    u.{nameof(PKey.Id).ToSnake()} AS {nameof(EventItemPrivateModel.OwnerId)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(EventItemPrivateModel.OwnerFirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(EventItemPrivateModel.OwnerLastName)},
    user_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemPrivateModel.OwnerMainHash)},
    user_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemPrivateModel.OwnerThumbnailHash)},
    user_likes.likes AS {nameof(EventItemPrivateModel.Likes)},
    user_likes.dislikes AS {nameof(EventItemPrivateModel.Dislikes)},
    user_likes.current_user_state AS {nameof(EventItemPrivateModel.OwnLikeState)},
    (SELECT EXISTS(
        SELECT 1 FROM {nameof(BaseDbContext.SavedEvents).ToSnake()} se 
                 WHERE se.{nameof(SavedEvent.EventId).ToSnake()} = e.{nameof(PKey.Id).ToSnake()} AND
                       se.{nameof(SavedEvent.UserId).ToSnake()} = @created_by_id)
     ) AS {nameof(EventItemPrivateModel.IsSavedByUser)},
    es.saved_count AS {nameof(EventItemPrivateModel.SavedCount)}
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
WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
     ({activeStatusFilter})
    AND
    ({accessFilter})
    AND
    LOWER(e.{nameof(Event.Name).ToSnake()}) ILIKE '%' || @search || '%'
    AND
    ({innerFilter})
ORDER BY e.{nameof(Event.CreatedAt).ToSnake()} DESC
OFFSET @offset
LIMIT @size;
";

            var countQuery = $@"
SELECT COUNT(1)
FROM {nameof(BaseDbContext.Events).ToSnake()} e 
WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
    ({activeStatusFilter})
    AND
    ({accessFilter})
    AND
    e.{nameof(Event.Name).ToSnake()} ILIKE '%' || @search || '%'
    AND
    ({innerFilter});
";
            await using var connection = _dapper.InitConnection();

            var count = await connection.QuerySingleAsync<int>(
                countQuery,
                new
                {
                    created_by_id = request.CurrentUserId,
                    status = (int)request.Status,
                    access = (int)request.Access,
                    search = request.Search
                });

            var events = await connection.QueryAsync<EventItemPrivateModel>(
                query,
                new
                {
                    created_by_id = request.CurrentUserId,
                    status = (int)request.Status,
                    access = (int)request.Access,
                    size = request.Size,
                    offset = request.Offset,
                    search = request.Search
                });
            
            return Result.Success(new GetEventsPaginationQueryResult
            {
                Count = count,
                Events = events.ToList()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetEventsPaginationQueryResult>(
                new Error(ErrorType.Event, $"Error while executing {nameof(GetEventsPaginationQuery)}"));
        }
    }
}