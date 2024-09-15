namespace How.Core.CQRS.Queries.Public.Record.GetRecordsPaginationPublic;

using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.Record;
using Database.Entities.Storage;
using DTO.Models;
using DTO.Record;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;

public class GetRecordsPaginationPublicQueryHandler : IQueryHandler<GetRecordsPaginationPublicQuery, Result<GetRecordsPaginationResponseDTO>>
{
    private readonly ILogger<GetRecordsPaginationPublicQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetRecordsPaginationPublicQueryHandler(ILogger<GetRecordsPaginationPublicQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetRecordsPaginationResponseDTO>> Handle(
        GetRecordsPaginationPublicQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    r.{nameof(PKey.Id).ToSnake()} AS {nameof(RecordItemModelDTO.Id)},
    r.{nameof(Record.Description).ToSnake()} AS {nameof(RecordItemModelDTO.Description)},
    r.{nameof(Record.CreatedAt).ToSnake()} AS {nameof(RecordItemModelDTO.CreatedAt)},
    user_likes.likes AS {nameof(RecordItemModelDTO.Likes)},
    user_likes.dislikes AS {nameof(RecordItemModelDTO.Dislikes)},
    user_likes.current_user_state AS {nameof(RecordItemModelDTO.OwnLikeState)},
    sf_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(ImageModelDTO.MainHash)},
    sf_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(ImageModelDTO.ThumbnailHash)}
FROM (
    SELECT rec.*
    FROM {nameof(BaseDbContext.Records).ToSnake()} rec
    LEFT JOIN {nameof(BaseDbContext.Events).ToSnake()} e ON rec.{nameof(Record.EventId).ToSnake()} = e.{nameof(PKey.Id).ToSnake()}
    WHERE e.{nameof(PKey.Id).ToSnake()} = @eventId
        AND
        e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
        AND
        e.{nameof(Event.Status).ToSnake()} = @status
        AND
        e.{nameof(Event.Access).ToSnake()} = @access
    ORDER BY rec.{nameof(Record.CreatedAt).ToSnake()} DESC
    OFFSET @offset
    LIMIT @size
    ) r
LEFT JOIN {nameof(BaseDbContext.RecordImages).ToSnake()} ri ON r.{nameof(PKey.Id).ToSnake()} = ri.{nameof(RecordImage.RecordId).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageImages).ToSnake()} si ON ri.{nameof(RecordImage.ImageId).ToSnake()} = si.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} sf_main ON si.{nameof(StorageImage.MainId).ToSnake()} = sf_main.{nameof(PKey.Id).ToSnake()}
LEFT JOIN {nameof(BaseDbContext.StorageFiles).ToSnake()} sf_thumbnail ON si.{nameof(StorageImage.ThumbnailId).ToSnake()} = sf_thumbnail.{nameof(PKey.Id).ToSnake()}
LEFT JOIN (
        SELECT 
            lr.{nameof(LikedRecord.RecordId).ToSnake()} AS liked_record_id,
            COUNT(CASE WHEN lr.{nameof(LikedRecord.State).ToSnake()} = 2 THEN 1 END) AS likes,
            COUNT(CASE WHEN lr.{nameof(LikedRecord.State).ToSnake()} = 3 THEN 1 END) AS dislikes,
            coalesce(lr_u.{nameof(LikedRecord.State).ToSnake()}, 1) AS current_user_state
        FROM {nameof(BaseDbContext.LikedRecords).ToSnake()} lr 
        LEFT JOIN {nameof(BaseDbContext.LikedRecords).ToSnake()} lr_u ON 
            lr_u.{nameof(LikedRecord.RecordId).ToSnake()} = lr.{nameof(LikedRecord.RecordId).ToSnake()} AND 
            lr_u.{nameof(LikedRecord.LikedByUserId).ToSnake()} = @created_by_id
        GROUP BY lr.{nameof(LikedRecord.RecordId).ToSnake()}, lr_u.{nameof(LikedRecord.State).ToSnake()}) user_likes ON r.{nameof(PKey.Id).ToSnake()} = user_likes.liked_record_id;
";

            var countQuery = $@"
SELECT COUNT(1)
FROM {nameof(BaseDbContext.Records).ToSnake()} r
LEFT JOIN {nameof(BaseDbContext.Events).ToSnake()} e ON r.{nameof(Record.EventId).ToSnake()} = e.{nameof(PKey.Id).ToSnake()}
WHERE e.{nameof(PKey.Id).ToSnake()} = @eventId
    AND
    e.{nameof(Event.IsDeleted).ToSnake()} = FALSE
    AND
    e.{nameof(Event.Status).ToSnake()} = @status
    AND
    e.{nameof(Event.Access).ToSnake()} = @access;
";
            await using var connection = _dapper.InitConnection();
            
            var count = await connection.QuerySingleAsync<int>(
                countQuery,
                new
                {
                    status = (int)EventStatus.Active,
                    access = (int)EventAccessType.Public,
                    eventId = request.EventId
                });

            if (count == 0)
            {
                return Result.Success(new GetRecordsPaginationResponseDTO());
            }
            
            var recordDictionary = new Dictionary<int, RecordItemModelDTO>(count);

            await connection.QueryAsync<RecordItemModelDTO, ImageModelDTO, RecordItemModelDTO>(
                query,
                (record, image) =>
                {
                    var currentRecord = recordDictionary.GetValueOrDefault(record.Id, record);

                    if (image is not null)
                    {
                        currentRecord.Images.Add(new ImageModelDTO
                        {
                            MainHash = image.MainHash,
                            ThumbnailHash = image.ThumbnailHash
                        });
                    }

                    recordDictionary.TryAdd(currentRecord.Id, currentRecord);
                    
                    return null;
                },
                param: new
                {
                    created_by_id = request.CurrentUserId,
                    status = (int)EventStatus.Active,
                    access = (int)EventAccessType.Public,
                    eventId = request.EventId,
                    size = request.Size,
                    offset = request.Offset
                },
                splitOn: $"id,{nameof(ImageModelDTO.MainHash)}");

            return Result.Success(new GetRecordsPaginationResponseDTO
            {
                Count = count,
                Records = recordDictionary.Values.ToArray()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetRecordsPaginationResponseDTO>(
                new Error(ErrorType.Record, $"Error while executing {nameof(GetRecordsPaginationPublicQuery)}"));
        }
    }
}