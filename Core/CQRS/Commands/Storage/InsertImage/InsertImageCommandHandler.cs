namespace How.Core.CQRS.Commands.Storage.InsertImage;

using Common.CQRS;
using Common.ResultType;
using Dapper;
using Database;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Npgsql;

public class InsertImageCommandHandler : ICommandHandler<InsertImageCommand, Result<int>>
{
    private readonly ILogger<InsertImageCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public InsertImageCommandHandler(ILogger<InsertImageCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int>> Handle(
        InsertImageCommand request, 
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
                return Result.Failure<int>(new Error(ErrorType.Storage,$"Error while executing {nameof(InsertImageCommand)}"));
            }
            
            var command = $@"
INSERT INTO storage_images (image_height, image_width, thumbnail_height, thumbnail_width, main_id, thumbnail_id) 
VALUES (@image_height, @image_width, @thumbnail_height, @thumbnail_width, @main_id, @thumbnail_id) 
RETURNING id;
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

            await transaction.CommitAsync(CancellationToken.None);
            return Result.Success(result);
            
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(e.Message);
            return Result.Failure<int>(
                new Error(ErrorType.Account, $"Error while executing {nameof(InsertImageCommand)}"));
        }
    }

    private async Task<int> InsertFile(FileInternalModel file, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        var command = $@"
INSERT INTO storage_files (hash, name, path, extension, size, content) 
VALUES (@hash, @name, @path, @extension, @size, @content) 
RETURNING id;
";
        var result = await connection.QuerySingleAsync<int>(
            command, 
            new
            {
                hash = file.Hash,
                name = file.Name,
                path = file.Path,
                extensions = file.Extension,
                size = file.Size,
                content = file.Content
            }, transaction);

        return result;
    }
}