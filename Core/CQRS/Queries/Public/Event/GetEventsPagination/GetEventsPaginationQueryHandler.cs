namespace How.Core.CQRS.Queries.Public.Event.GetEventsPagination;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Event;
using Database.Entities.Identity;
using Database.Entities.Storage;
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
            var query = $@"
SELECT 
    e.id AS {nameof(EventItemModel.Id)},
    e.{nameof(Event.Name).ToSnake()} AS {nameof(EventItemModel.Name)},
    e.{nameof(Event.CreatedAt).ToSnake()} AS {nameof(EventItemModel.CreatedAt)},
    e.{nameof(Event.Access).ToSnake()} As {nameof(EventItemModel.Access)},
    event_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemModel.EventMainHash)},
    event_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemModel.EventThumbnailHash)},
    u.id AS {nameof(EventItemModel.OwnerId)},
    u.{nameof(HowUser.FirstName).ToSnake()} AS {nameof(EventItemModel.OwnerFirstName)},
    u.{nameof(HowUser.LastName).ToSnake()} AS {nameof(EventItemModel.OwnerLastName)},
    user_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemModel.OwnerMainHash)},
    user_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(EventItemModel.OwnerThumbnailHash)}
FROM {nameof(BaseDbContext.Events).ToSnake()} e 
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} event_image ON 
    e.{nameof(Event.StorageImageId).ToSnake()} = event_image.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_main ON 
    event_image.{nameof(StorageImage.MainId).ToSnake()} = event_main.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} event_thumbnail ON 
    event_image.{nameof(StorageImage.ThumbnailId).ToSnake()} = event_thumbnail.id
LEFT JOIN {nameof(BaseDbContext.Users).ToSnake()} u ON 
    e.{nameof(Event.OwnerId).ToSnake()} = u.id
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} user_image ON 
    u.{nameof(HowUser.StorageImageId).ToSnake()} = user_image.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} user_main ON 
    user_image.{nameof(StorageImage.MainId).ToSnake()} = user_main.id
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} user_thumbnail ON 
    user_image.{nameof(StorageImage.ThumbnailId).ToSnake()} = user_thumbnail.id
WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
    e.{nameof(Event.Status).ToSnake()} = @status
    AND
    e.{nameof(Event.Access).ToSnake()} = @access
    AND
    LOWER(e.{nameof(Event.Name).ToSnake()}) ILIKE '%' || @search || '%'
ORDER BY e.{nameof(Event.CreatedAt).ToSnake()} DESC
OFFSET @offset
LIMIT @size;
";

            var countQuery = $@"
SELECT COUNT(1)
FROM {nameof(BaseDbContext.Events).ToSnake()} e 
WHERE e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
    e.{nameof(Event.Status).ToSnake()} = @status
    AND
    e.{nameof(Event.Access).ToSnake()} = @access
    AND
    e.{nameof(Event.Name).ToSnake()} ILIKE '%' || @search || '%'
";
            await using var connection = _dapper.InitConnection();

            var count = await connection.QuerySingleAsync<int>(
                countQuery,
                new
                {
                    status = EventStatus.Active,
                    access = EventAccessType.Public,
                    search = request.Search
                });

            var events = await connection.QueryAsync<EventItemModel>(
                query,
                new
                {
                    status = EventStatus.Active,
                    access = EventAccessType.Public,
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