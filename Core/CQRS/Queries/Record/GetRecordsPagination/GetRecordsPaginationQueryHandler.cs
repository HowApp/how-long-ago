namespace How.Core.CQRS.Queries.Record.GetRecordsPagination;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Base;
using Database.Entities.Record;
using Database.Entities.Storage;
using DTO.Models;
using DTO.Record;
using Microsoft.Extensions.Logging;

public class GetRecordsPaginationQueryHandler : IQueryHandler<GetRecordsPaginationQuery, Result<GetRecordsPaginationResponseDTO>>
{
    private readonly ILogger<GetRecordsPaginationQueryHandler> _logger;
    private readonly DapperConnection _dapper;

    public GetRecordsPaginationQueryHandler(ILogger<GetRecordsPaginationQueryHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<GetRecordsPaginationResponseDTO>> Handle(
        GetRecordsPaginationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $@"
SELECT 
    r.{nameof(PKey.Id).ToSnake()} AS {nameof(RecordItemPrivateModelDTO.Id)},
    r.{nameof(Record.Description).ToSnake()} AS {nameof(RecordItemPrivateModelDTO.Description)},
    r.{nameof(Record.CreatedAt).ToSnake()} AS {nameof(RecordItemPrivateModelDTO.CreatedAt)},
    coalesce(user_likes.likes, 0) AS {nameof(RecordItemPrivateModelDTO.Likes)},
    coalesce(user_likes.dislikes, 0) AS {nameof(RecordItemPrivateModelDTO.Dislikes)},
    coalesce(user_likes.current_user_state, 1) AS {nameof(RecordItemPrivateModelDTO.OwnLikeState)},
    sf_main.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(ImageModelDTO.MainHash)},
    sf_thumbnail.{nameof(StorageFile.Hash).ToSnake()} AS {nameof(ImageModelDTO.ThumbnailHash)},
    coalesce(ri.{nameof(PKey.Id).ToSnake()}, 0) AS {nameof(ImageModelDTO.Id)}
FROM (
    SELECT *
    FROM {nameof(BaseDbContext.Records).ToSnake()} rec
    WHERE rec.{nameof(Record.EventId).ToSnake()} = @eventId
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
            coalesce(MAX(CASE WHEN lr.{nameof(LikedRecord.LikedByUserId).ToSnake()} = @createdById THEN lr.{nameof(LikedRecord.State).ToSnake()} END), 1) AS current_user_state
        FROM {nameof(BaseDbContext.LikedRecords).ToSnake()} lr 
        GROUP BY lr.{nameof(LikedRecord.RecordId).ToSnake()}) user_likes ON r.{nameof(PKey.Id).ToSnake()} = user_likes.liked_record_id
ORDER BY r.{nameof(Record.CreatedAt).ToSnake()} DESC, ri.{nameof(RecordImage.Position).ToSnake()} ASC;
";

            var countQuery = $@"
SELECT COUNT(1)
FROM {nameof(BaseDbContext.Records).ToSnake()} r
WHERE r.{nameof(Record.EventId).ToSnake()} = @eventId;
";
            await using var connection = _dapper.InitConnection();

            var count = await connection.QuerySingleAsync<int>(
                countQuery,
                new
                {
                    eventId = request.EventId
                });

            if (count == 0)
            {
                return Result.Success(new GetRecordsPaginationResponseDTO());
            }

            var recordDictionary = new Dictionary<int, RecordItemPrivateModelDTO>(count);

            await connection.QueryAsync<RecordItemPrivateModelDTO, ImageModelDTO, RecordItemPrivateModelDTO>(
                query,
                (record, image) =>
                {
                    var currentRecord = recordDictionary.GetValueOrDefault(record.Id, record);

                    if (image is not null && !string.IsNullOrEmpty(image.MainHash))
                    {
                        currentRecord.Images.Add(new ImageModelDTO
                        {
                            Id = image.Id,
                            MainHash = image.MainHash,
                            ThumbnailHash = image.ThumbnailHash
                        });
                    }

                    recordDictionary.TryAdd(currentRecord.Id, currentRecord);

                    return null;
                },
                param: new
                {
                    createdById = request.CurrentUserId,
                    eventId = request.EventId,
                    size = request.Size,
                    offset = request.Offset
                },
                splitOn: nameof(ImageModelDTO.MainHash));

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
                new Error(ErrorType.Record, $"Error while executing {nameof(GetRecordsPaginationQuery)}"));
        }
    }
}