namespace How.Core.Services.Storage.FileStorage;

using System.Net;
using Common.Helpers;
using Common.ResultType;
using Database;
using Database.Entities.Storage;
using DTO.Storage.FileService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;

public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly BaseDbContext _dbContext;

    public FileStorageService(ILogger<FileStorageService> logger, BaseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result> PostFileToDatabase(IFormFile file)
    {
        try
        {
            var item = new FileStorage();

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
                item.Size = memoryStream.Length;
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                file.FileName);

            var extensions = Path.GetExtension(trustedFileNameForDisplay).ToLowerInvariant();
            var fileHash = HashHelper.ComputeMd5($"{SystemClock.Instance.GetCurrentInstant()}-{trustedFileNameForDisplay}");

            item.Hash = fileHash;
            item.Name = trustedFileNameForDisplay;
            item.Path = StoragePathHelper.Files.File(trustedFileNameForDisplay);
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
                $"Error while executing {nameof(PostFileToDatabase)}"));
        }
    }

    public async Task<Result<GetFileFromDatabaseByteResponseDTO>> GetFileFromDatabaseByte(string fileHash)
    {
        try
        {
            var image = await _dbContext.AppFiles
                .Where(i => i.Hash == fileHash)
                .Select(i => new
                {
                    FileName = i.Name,
                    i.Extension,
                    i.Content
                })
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Result.Failure<GetFileFromDatabaseByteResponseDTO>(new Error(
                    ErrorType.Storage,
                    $"File not found!"));
            }
            
            var result = new GetFileFromDatabaseByteResponseDTO
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
            return Result.Failure<GetFileFromDatabaseByteResponseDTO>(new Error(
                ErrorType.Storage,
                $"Error while executing {nameof(GetFileFromDatabaseByte)}"));
        }
    }

    public async Task<Result<GetFileFromDatabaseStreamResponseDTO>> GetFileFromDatabaseStream(string fileHash)
    {
        try
        {
            var image = await _dbContext.AppFiles
                .Where(i => i.Hash == fileHash)
                .Select(i => new
                {
                    FileName = i.Name,
                    i.Extension,
                    i.Content
                })
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Result.Failure<GetFileFromDatabaseStreamResponseDTO>(new Error(
                    ErrorType.Storage,
                    $"File not found!"));
            }

            var memoryStream = new MemoryStream(image.Content);
            var result = new GetFileFromDatabaseStreamResponseDTO
            {
                FileName = image.FileName,
                MimeType = CommonMIMETypesHelper.GetMIMEType(image.Extension),
                Content = memoryStream
            };
            
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result.Failure<GetFileFromDatabaseStreamResponseDTO>(new Error(
                ErrorType.Storage,
                $"Error while executing {nameof(GetFileFromDatabaseStream)}"));
        }
    }
}