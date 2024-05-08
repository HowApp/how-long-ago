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
using NodaTime;

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
            await using var originalStream = new MemoryStream();
            await file.CopyToAsync(originalStream);

            // Check the content length in case the file's only
            // content was a BOM and the content is actually
            // empty after removing the BOM.
            if (originalStream.Length == 0)
            {
                return Result.Failure(new Error(
                    ErrorType.Storage,
                    $"File is empty!"));
            }

            originalStream.Position = 0;

            var convertedImage = ImageHelper.ConvertImageToWebp(originalStream);
            
            await using var convertedStream = new MemoryStream(convertedImage.ImageData);
            var reducedImage = ImageHelper.GetReducedImage(convertedStream);
            
            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var extensions = AppFileTypeHelper.GetFileTypeFromExtensions(AppFileExt.WEBP);
            
            var trustedImageNameForDisplay = $"{WebUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName))}.{extensions}";
            var trustedThumbnailNameForDisplay = $"thumbnail-{trustedImageNameForDisplay}";
            
            var imageHash = HashHelper.ComputeMd5($"{SystemClock.Instance.GetCurrentInstant()}-{trustedImageNameForDisplay}");
            var thumbnailHash = HashHelper.ComputeMd5($"{SystemClock.Instance.GetCurrentInstant()}-{trustedThumbnailNameForDisplay}");
            
            var item = new Image
            {
                ImageHeight = convertedImage.Height,
                ImageWidth = convertedImage.Width,
                ThumbnailHeight = reducedImage.Height,
                ThumbnailWidth = reducedImage.Width,
                Main = new FileStorage
                {
                    Hash = imageHash,
                    Name = trustedImageNameForDisplay,
                    Path = StoragePathHelper.Images.Image(trustedImageNameForDisplay),
                    Extension = extensions,
                    Size = convertedImage.ImageData.Length,
                    Content = convertedImage.ImageData
                },
                Thumbnail = new FileStorage
                {
                    Hash = thumbnailHash,
                    Name = trustedThumbnailNameForDisplay,
                    Path = StoragePathHelper.Images.Image(trustedThumbnailNameForDisplay),
                    Extension = extensions,
                    Size = reducedImage.ImageData.Length,
                    Content = reducedImage.ImageData
                }
            };
            
            _dbContext.StorageImages.Add(item);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            GC.Collect();
            
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