namespace How.Common.Validators;

using Enums;
using Microsoft.AspNetCore.Http;

public static class FileSignatureValidator
{
    private static readonly Dictionary<AppFileExt, List<byte[]>> ImageSignature = new()
    {
        {
            AppFileExt.PNG, new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
            }
        },
        {
            AppFileExt.JPEG, new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF }
            }
        },
        {
            AppFileExt.JPG, new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF }
            }
        },
        {
            // Google WebP image file, where second item 00 00 00 00 is the file size
            AppFileExt.WEBP, new List<byte[]>
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50 }
            }
        },
    };

    public static bool IsValidImageExtensionAndSignature(IFormFile file, AppFileExt[] permittedExtensions)
    {
        using var data = file.OpenReadStream();
        data.Position = 0;

        using (var reader = new BinaryReader(data, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            var firstBytes = reader.ReadBytes(100);

            foreach (var ext in permittedExtensions)
            {
                var val = false;

                if (!ImageSignature.TryGetValue(ext, out var signature))
                {
                    continue;
                }

                var headerByte = firstBytes.Take(signature.Max(m => m.Length)).ToArray();

                if (ext == AppFileExt.WEBP)
                {
                    val = headerByte.Length >= 12 &&
                          signature.Any(s => 
                              s.Take(4).SequenceEqual(headerByte.Take(4)) && 
                              s.Skip(8).Take(4).SequenceEqual(headerByte.Skip(8).Take(4)));
                }
                else
                {
                    val = signature.Any(s => headerByte.Length >= s.Length && headerByte.Take(s.Length).SequenceEqual(s));
                }

                if (val)
                {
                    return true;
                }
            }

            return false;
        }
    }
}