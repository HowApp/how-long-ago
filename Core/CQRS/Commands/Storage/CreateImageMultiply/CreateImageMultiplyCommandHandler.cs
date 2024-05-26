namespace How.Core.CQRS.Commands.Storage.CreateImageMultiply;

using System.Text;
using Common.CQRS;
using Common.Extensions;
using Common.ResultType;
using Dapper;
using Database;
using Database.Entities.Storage;
using Microsoft.Extensions.Logging;
using Models.ServicesModel;
using Npgsql;

public class CreateImageMultiplyCommandHandler : ICommandHandler<CreateImageMultiplyCommand, Result<int[]>>
{
    private readonly ILogger<CreateImageMultiplyCommandHandler> _logger;
    private readonly DapperConnection _dapper;

    public CreateImageMultiplyCommandHandler(ILogger<CreateImageMultiplyCommandHandler> logger, DapperConnection dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<Result<int[]>> Handle(CreateImageMultiplyCommand request, CancellationToken cancellationToken)
    {
        await using var connection = _dapper.InitConnection();
        await using var transaction = await connection.BeginTransactionAsync(CancellationToken.None);
        
        try
        {
            var mainId = await InsertFile(request.Images.Select(i => i.Main).ToList(), connection, transaction);
            var thumbnailId = await InsertFile(request.Images.Select(i => i.Thumbnail).ToList(), connection, transaction);

            if (!mainId.Any() || !thumbnailId.Any())
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError($"Error while insert {nameof(StorageFile)} at {nameof(CreateImageMultiplyCommand)}");
                return Result.Failure<int[]>(
                    new Error(ErrorType.Account, $"Error while insert {nameof(StorageFile)} at {nameof(CreateImageMultiplyCommand)}"));
            }
            
            var sql = new StringBuilder();
            var replacedItem = "@image_height, @image_width, @thumbnail_height, @thumbnail_width, @main_id, @thumbnail_id";
            
            var command = @$"
INSERT INTO {nameof(BaseDbContext.StorageImages).ToSnake()} (
    {nameof(StorageImage.ImageHeight).ToSnake()},
    {nameof(StorageImage.ImageWidth).ToSnake()},
    {nameof(StorageImage.ThumbnailHeight).ToSnake()},
    {nameof(StorageImage.ThumbnailWidth).ToSnake()},
    {nameof(StorageImage.MainId).ToSnake()},
    {nameof(StorageImage.ThumbnailId).ToSnake()}) 
VALUES 
    ({replacedItem}) 
RETURNING {nameof(StorageImage.Id).ToSnake()};
";
            sql.Append(command);
            
            var values = new List<string>();
            var parameters = new DynamicParameters();
            
            for (int i = 0; i < request.Images.Count; i++)
            {
                values.Add(@$"(@image_height_{i}, @image_width_{i}, @thumbnail_height_{i}, @thumbnail_width_{i}, @main_id_{i}, @thumbnail_id_{i})");
                
                parameters.AddDynamicParams(
                    new Dictionary<string, object>
                    {
                        { $"@image_height_{i}", request.Images[i].ImageHeight},
                        { $"@image_width_{i}", request.Images[i].ImageWidth},
                        { $"@thumbnail_height_{i}", request.Images[i].ThumbnailHeight},
                        { $"@thumbnail_width_{i}", request.Images[i].ThumbnailWidth},
                        { $"@main_id_{i}", mainId[i]},
                        { $"@thumbnail_id_{i}", thumbnailId[i]},
                    });
            }
            
            sql.Replace($"({replacedItem})", string.Join(", \n", values));
            
            var result = (await connection.QueryAsync<int>(sql.ToString(), parameters, transaction)).ToArray();
            
            if (!result.Any())
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogError($"Error while insert {nameof(StorageImage)} at {nameof(CreateImageMultiplyCommand)}");
                return Result.Failure<int[]>(
                    new Error(ErrorType.Account, $"Error while insert {nameof(StorageImage)} at {nameof(CreateImageMultiplyCommand)}"));
            }
            
            await transaction.CommitAsync(CancellationToken.None);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(e.Message);
            return Result.Failure<int[]>(
                new Error(ErrorType.Account, $"Error while executing {nameof(CreateImageMultiplyCommand)}"));
        }
    }
    
    private async Task<int[]> InsertFile(List<FileInternalModel> files, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        var sql = new StringBuilder();
        var replacedItem = "@hash, @name, @path, @extension, @size, @content";
        
        var command = $@"
INSERT INTO {nameof(BaseDbContext.StorageFiles).ToSnake()} (
    {nameof(StorageFile.Hash).ToSnake()},
    {nameof(StorageFile.Name).ToSnake()},
    {nameof(StorageFile.Path).ToSnake()},
    {nameof(StorageFile.Extension).ToSnake()},
    {nameof(StorageFile.Size).ToSnake()},
    {nameof(StorageFile.Content).ToSnake()}) 
VALUES ({replacedItem})
RETURNING {nameof(StorageFile.Id).ToSnake()};
";
        sql.Append(command);
        
        var values = new List<string>();
        var parameters = new DynamicParameters();

        for (int i = 0; i < files.Count; i++)
        {
            values.Add(@$"(@hash_{i}, @name_{i}, @path_{i}, @extension_{i}, @size_{i}, @content_{i})");
            
            parameters.AddDynamicParams(
                new Dictionary<string, object>
                {
                    { $"@hash_{i}", files[i].Hash},
                    { $"@name_{i}", files[i].Name},
                    { $"@path_{i}", files[i].Path},
                    { $"@extension_{i}", files[i].Extension},
                    { $"@size_{i}", files[i].Size},
                    { $"@content_{i}", files[i].Content},
                });
        }

        sql.Replace($"({replacedItem})", string.Join(", \n", values));
        
        var result = await connection.QueryAsync<int>(sql.ToString(), parameters, transaction);

        return result.ToArray();
    }
}