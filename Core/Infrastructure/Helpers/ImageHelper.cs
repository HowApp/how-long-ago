namespace How.Core.Infrastructure.Helpers;

using Common.Constants;
using Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

public static class ImageHelper
{
    private const int ThumbnailResolution = AppConstants.Images.ThumbnailResolution;
    
    private static WebpEncoder Encoder() => new WebpEncoder
    {
        SkipMetadata = false,
        FileFormat = WebpFileFormatType.Lossy,
        Quality = 50,
        Method = WebpEncodingMethod.Fastest,
    };
    
    public static ImageHelperModel GetReducedImage(Stream resourceImage, int resizedWidth = ThumbnailResolution)
    {
        using var outStream = new MemoryStream();
        using var image = Image.Load(resourceImage);
        
        float nPercent;
        var width = image.Width;
        var height = image.Height;

        IExifValue<ushort> exifOrientation = null;

        image.Metadata.ExifProfile?.TryGetValue(ExifTag.Orientation, out exifOrientation);

        if (exifOrientation?.Value is > 4 and <=8)
        {
            if (height < resizedWidth)
            {
                image.Save(outStream, Encoder());
                return new ImageHelperModel
                {
                    ImageData = outStream.ToArray(),
                    Width = image.Width,
                    Height = image.Height,
                };
            }

            nPercent = resizedWidth / (float)height;
        }
        else
        {
            if (width < resizedWidth)
            {
                image.Save(outStream, Encoder());
                return new ImageHelperModel
                {
                    ImageData = outStream.ToArray(),
                    Width = image.Width,
                    Height = image.Height,
                };
            }

            nPercent = resizedWidth / (float)width;
        }

        var destWidth = (int)(width * nPercent);
        var destHeight = (int)(height * nPercent);

        image.Mutate(i => i.Resize(destWidth, destHeight));
        image.Save(outStream, Encoder());

        return new ImageHelperModel
        {
            ImageData = outStream.ToArray(),
            Width = image.Width,
            Height = image.Height,
        };
    }

    public static (int Width, int Height) GetImageResolution(Stream resourceImage)
    {
        using var image = Image.Load(resourceImage);

        var width = image.Width;
        var height = image.Height;

        return (width, height);
    }

    public static ImageHelperModel ConvertImageToWebp(Stream resourceImage)
    {
        var outStream = new MemoryStream();
        using var image = Image.Load(resourceImage);

        image.Save(outStream, Encoder());

        return new ImageHelperModel
        {
            ImageData = outStream.ToArray(),
            Width = image.Width,
            Height = image.Height
        };
    }
}