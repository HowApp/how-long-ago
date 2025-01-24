namespace How.Core.CQRS.Queries.Public.Record.GetRecordsPaginationPublic;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Event;
using Database.Entities.Record;
using Database.Entities.Storage;
using DTO.Models;
using DTO.Public.Record;
using Infrastructure.Enums;
using Microsoft.Extensions.Logging;

public class GetRecordsPaginationPublicQueryHandler : IQueryHandler<GetRecordsPaginationPublicQuery, Result<GetRecordsPaginationPublicResponseDTO>>
{
    private readonly ILogger<GetRecordsPaginationPublicQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetRecordsPaginationPublicQueryHandler(ILogger<GetRecordsPaginationPublicQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetRecordsPaginationPublicResponseDTO>> Handle(
        GetRecordsPaginationPublicQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    r.{nameof(PKey.Id).ToSnake()} AS {nameof(RecordItemPublicModelDTO.Id)},
    r.{nameof(Record.Description).ToSnake()} AS {nameof(RecordItemPublicModelDTO.Description)},
    r.{nameof(Record.CreatedAt).ToSnake()} AS {nameof(RecordItemPublicModelDTO.CreatedAt)},
    user_likes.likes AS {nameof(RecordItemPublicModelDTO.Likes)},
    user_likes.dislikes AS {nameof(RecordItemPublicModelDTO.Dislikes)},
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
            COUNT(CASE WHEN lr.{nameof(LikedRecord.State).ToSnake()} = 3 THEN 1 END) AS dislikes
        FROM {nameof(BaseDbContext.LikedRecords).ToSnake()} lr 
        GROUP BY lr.{nameof(LikedRecord.RecordId).ToSnake()}) user_likes ON r.{nameof(PKey.Id).ToSnake()} = user_likes.liked_record_id
ORDER BY r.{nameof(Record.CreatedAt).ToSnake()} DESC;
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
                return Result.Success(new GetRecordsPaginationPublicResponseDTO());
            }
            
            var recordDictionary = new Dictionary<int, RecordItemPublicModelDTO>(count);

            await connection.QueryAsync<RecordItemPublicModelDTO, ImageModelDTO, RecordItemPublicModelDTO>(
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
                    status = (int)EventStatus.Active,
                    access = (int)EventAccessType.Public,
                    eventId = request.EventId,
                    size = request.Size,
                    offset = request.Offset
                },
                splitOn: $"id,{nameof(ImageModelDTO.MainHash)}");

            return Result.Success(new GetRecordsPaginationPublicResponseDTO
            {
                Count = count,
                Records = recordDictionary.Values.ToArray()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetRecordsPaginationPublicResponseDTO>(
                new Error(ErrorType.Record, $"Error while executing {nameof(GetRecordsPaginationPublicQuery)}"));
        }
    }
}