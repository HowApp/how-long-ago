namespace How.Core.Services.Storage;

using System.Net;
using Common.Helpers;
using Common.ResultType;
using Database;
using Database.Entities.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.ServicesModel.StorageService;

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly BaseDbContext _dbContext;

    public StorageService(ILogger<StorageService> logger, BaseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result> PostImageToDatabase(IFormFile file)
    {
        try
        {
            var item = new AppFile();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                // Check the content length in case the file's only
                // content was a BOM and the content is actually
                // empty after removing the BOM.
                if (memoryStream.Length == 0)
                {
                    return Result.Failure(new Error(
                        ErrorType.Storage,
                        $"File is empty!"));
                }

                item.Content = memoryStream.ToArray();
                item.FileSize = memoryStream.Length;
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                file.FileName);

            var extensions = Path.GetExtension(trustedFileNameForDisplay).ToLowerInvariant();
            var fileHash = HashHelper.ComputeMd5($"{DateTime.UtcNow}-{trustedFileNameForDisplay}");

            item.FileHash = fileHash;
            item.FileName = trustedFileNameForDisplay;
            item.Extension = extensions;


            _dbContext.AppFiles.Add(item);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure(new Error(
                ErrorType.Storage,
                $"Error while executing {nameof(PostImageToDatabase)}"));
        }
    }

    public async Task<Result<GetImageFromDatabaseByteResult>> GetImageFromDatabaseByte(string fileHash)
    {
        try
        {
            var image = await _dbContext.AppFiles
                .Where(i => i.FileHash == fileHash)
                .Select(i => new
                {
                    i.FileName,
                    i.Extension,
                    i.Content
                })
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Result.Failure<GetImageFromDatabaseByteResult>(new Error(
                    ErrorType.Storage,
                    $"File not found!"));
            }
            
            var result = new GetImageFromDatabaseByteResult
            {
                FileName = image.FileName,
                MimeType = CommonMIMETypesHelper.GetMIMEType(image.Extension),
                Content = image.Content
            };

            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetImageFromDatabaseByteResult>(new Error(
                ErrorType.Storage,
                $"Error while executing {nameof(GetImageFromDatabaseByte)}"));
        }
    }

    public async Task<Result<GetImageFromDatabaseStreamResult>> GetImageFromDatabaseStream(string fileHash)
    {
        try
        {
            var image = await _dbContext.AppFiles
                .Where(i => i.FileHash == fileHash)
                .Select(i => new
                {
                    i.Extension,
                    i.Content
                })
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Result.Failure<GetImageFromDatabaseStreamResult>(new Error(
                    ErrorType.Storage,
                    $"File not found!"));
            }

            var memoryStream = new MemoryStream(image.Content);
            var result = new GetImageFromDatabaseStreamResult
            {
                MimeType = CommonMIMETypesHelper.GetMIMEType(image.Extension),
                Content = memoryStream
            };
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetImageFromDatabaseStreamResult>(new Error(
                ErrorType.Storage,
                $"Error while executing {nameof(GetImageFromDatabaseStream)}"));
        }
    }
}