namespace How.Core.CQRS.Commands.Storage.CreateImage;

using Common.CQRS;
using HowCommon.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Npgsql;

public class CreateImageCommandHandler : ICommandHandler<CreateImageCommand, Result<int>>
{
    private readonly ILogger<CreateImageCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public CreateImageCommandHandler(ILogger<CreateImageCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(
        CreateImageCommand request, 
        CancellationToken cancellationToken)
    {
        await using var connection = _dapper.InitConnection();
        await using var transaction = await connection.BeginTransactionAsync(CancellationToken.None);
        
        try
        {
            var mainId = await InsertFile(request.Image.Main, connection, transaction);
            var thumbnailId = await InsertFile(request.Image.Thumbnail, connection, transaction);

            if (mainId < 1 || thumbnailId < 1)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError($"Error while insert {nameof(StorageFile)} at {nameof(CreateImageCommand)}");
                return Result.Failure<int>(
                    new Error(ErrorType.Account, $"Error while insert {nameof(StorageFile)} at {nameof(CreateImageCommand)}"));
            }
            
            var command = @$"
INSERT INTO {nameof(BaseDbContext.StorageImages).ToSnake()} (
    {nameof(StorageImage.ImageHeight).ToSnake()},
    {nameof(StorageImage.ImageWidth).ToSnake()},
    {nameof(StorageImage.ThumbnailHeight).ToSnake()},
    {nameof(StorageImage.ThumbnailWidth).ToSnake()},
    {nameof(StorageImage.MainId).ToSnake()},
    {nameof(StorageImage.ThumbnailId).ToSnake()}) 
VALUES (@image_height, @image_width, @thumbnail_height, @thumbnail_width, @main_id, @thumbnail_id) 
RETURNING {nameof(StorageImage.Id).ToSnake()};
";
            var result = await connection.QuerySingleAsync<int>(
                command, 
                new
                {
                    image_height = request.Image.ImageHeight,
                    image_width = request.Image.ImageWidth,
                    thumbnail_height = request.Image.ThumbnailHeight,
                    thumbnail_width = request.Image.ThumbnailWidth,
                    main_id = mainId,
                    thumbnail_id = thumbnailId
                }, transaction);

            if (result < 1)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError($"Error while insert {nameof(StorageImage)} at {nameof(CreateImageCommand)}");
                return Result.Failure<int>(
                    new Error(ErrorType.Account, $"Error while insert {nameof(StorageImage)} at {nameof(CreateImageCommand)}"));
            }
            
            await transaction.CommitAsync(CancellationToken.None);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(CreateImageCommand)}"));
        }
    }

    private async Task<int> InsertFile(FileInternalModel file, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        var command = $@"
INSERT INTO {nameof(BaseDbContext.StorageFiles).ToSnake()} (
    {nameof(StorageFile.Hash).ToSnake()},
    {nameof(StorageFile.Name).ToSnake()},
    {nameof(StorageFile.Path).ToSnake()},
    {nameof(StorageFile.Extension).ToSnake()},
    {nameof(StorageFile.Size).ToSnake()},
    {nameof(StorageFile.Content).ToSnake()}) 
VALUES (@hash, @name, @path, @extension, @size, @content) 
RETURNING {nameof(StorageFile.Id).ToSnake()};
";
        var result = await connection.QuerySingleAsync<int>(
            command, 
            new
            {
                hash = file.Hash,
                name = file.Name,
                path = file.Path,
                extension = file.Extension,
                size = file.Size,
                content = file.Content
            }, transaction);

        return result;
    }
}