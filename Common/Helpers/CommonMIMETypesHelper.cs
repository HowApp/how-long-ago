namespace How.Common.Helpers;

using Enums;

public static class CommonMIMETypesHelper
{
    private static readonly string DefaultMIMEType = "application/octet-stream";
    
    private static readonly Dictionary<string, string> MIMETypes = new Dictionary<string, string>()
    {
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".webp", "image/webp"},
        {".txt", "text/plain"},
    };

    public static string GetMIMEType(string ext)
    {
        if (!MIMETypes.TryGetValue(ext, out var mimeType))
        {
            return DefaultMIMEType;
        }

        return mimeType;
    }
}