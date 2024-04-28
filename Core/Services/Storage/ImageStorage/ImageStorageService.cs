namespace How.Core.Services.Storage.ImageStorage;

using System.Net;
using Common.Enums;
using Common.Helpers;
using Common.ResultType;
using Database;
using Database.Entities.Storage;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ImageStorageService : IImageStorageService
{
    private readonly ILogger<ImageStorageService> _logger;
    private readonly BaseDbContext _dbContext;

    public ImageStorageService(ILogger<ImageStorageService> logger, BaseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result> PostImageToDatabase(IFormFile file)
    {
        try
        {
            await using var memoryStream = new MemoryStream();
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

            var convertedImage = ImageHelper.ConvertImageToWebp(memoryStream.ToArray());
            var convertedImageResolution = ImageHelper.GetImageResolution(convertedImage);

            var reducedImage = ImageHelper.GetReducedImage(convertedImage);
            
            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var extensions = AppFileTypeHelper.GetFileTypeFromExtensions(AppFileExt.WEBP);
            var trustedImageNameForDisplay = $"{WebUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName))}.{extensions}";
            var trustedThumbnailNameForDisplay = $"thumbnail-{trustedImageNameForDisplay}";
            var imageHash = HashHelper.ComputeMd5($"{DateTime.UtcNow}-{trustedImageNameForDisplay}");
            var thumbnailHash = HashHelper.ComputeMd5($"{DateTime.UtcNow}-{trustedThumbnailNameForDisplay}");
            
            var item = new StorageImage
            {
                ImageHeight = convertedImageResolution.Height,
                ImageWidth = convertedImageResolution.Width,
                ThumbnailHeight = reducedImage.Height,
                ThumbnailWidth = reducedImage.Width,
                Image = new AppFile
                {
                    FileHash = imageHash,
                    FileName = trustedImageNameForDisplay,
                    Extension = extensions,
                    FileSize = convertedImage.Length,
                    Content = convertedImage
                },
                Thumbnail = new AppFile
                {
                    FileHash = thumbnailHash,
                    FileName = trustedThumbnailNameForDisplay,
                    Extension = extensions,
                    FileSize = reducedImage.ImageData.Length,
                    Content = reducedImage.ImageData
                }
            };
            
            _dbContext.StorageImages.Add(item);
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
}