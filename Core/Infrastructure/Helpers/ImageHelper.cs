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
    
    public static ReducedImageModel GetReducedImage(byte[] resourceImage, int resizedWidth = ThumbnailResolution)
    {
        using var outStream = new MemoryStream();
        using var image = Image.Load(resourceImage);
        var format = image.Metadata.DecodedImageFormat;

        float nPercent;
        var width = image.Width;
        var height = image.Height;

        IExifValue<ushort> exifOrientation = null;

        image.Metadata.ExifProfile?.TryGetValue(ExifTag.Orientation, out exifOrientation);

        if (exifOrientation?.Value is > 4 and <=8)
        {
            if (height < resizedWidth)
            {
                image.Save(outStream, format);
                return new ReducedImageModel
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
                image.Save(outStream, format);
                return new ReducedImageModel
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
        image.SaveAsWebp(outStream);

        return new ReducedImageModel
        {
            ImageData = outStream.ToArray(),
            Width = image.Width,
            Height = image.Height,
        };
    }

    public static (int Width, int Height) GetImageResolution(byte[] resourceImage)
    {
        using var image = Image.Load(resourceImage);

        var width = image.Width;
        var height = image.Height;

        return (width, height);
    }

    public static byte[] ConvertImageToWebp(byte[] resourceImage)
    {
        var format = Image.DetectFormat(resourceImage);

        if (format.DefaultMimeType != WebpFormat.Instance.DefaultMimeType)
        {
            var outStream = new MemoryStream();
            using var image = Image.Load(resourceImage);

            image.SaveAsWebp(outStream);

            var result = outStream.ToArray();

            outStream.Dispose();

            return result;
        }

        return resourceImage;
    }
}