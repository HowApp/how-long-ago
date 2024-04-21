namespace How.Common.Helpers;

using Enums;
using Exceptions;
using Extensions;

public static class AppFileTypeHelper
{
    private static readonly Dictionary<string, AppFileExt> Extensions; 

    private static readonly Dictionary<AppFileExt, string> ExtensionString;

    static AppFileTypeHelper()
    {
        Extensions = new()
        {
            { "png", AppFileExt.PNG },
            { "jpg", AppFileExt.JPG },
            { "jpeg", AppFileExt.JPEG },
            { "webp", AppFileExt.WEBP },
            { "txt", AppFileExt.TXT },
        };

        ExtensionString = Extensions.ReverseKEyValue();
    }

    public static AppFileExt GetFileTypeFromExtensions(string ext)
    {
        if (string.IsNullOrEmpty(ext))
        {
            throw new FileTypeException(new Dictionary<string, string>
            {
                { "File extension", $"Extension {ext} is null." }
            });
        }
        
        ext = ext.Replace(".", "").ToLower();
        
        if (!Extensions.TryGetValue(ext, out AppFileExt fileType))
        {
            throw new FileTypeException(new Dictionary<string, string>
            {
                { "File extension", $"Extension {ext} not allowed in App." }
            });
        }

        return fileType;
    }

    public static string GetFileTypeFromExtensions(AppFileExt ext)
    {
        if (!ExtensionString.TryGetValue(ext, out var fileType))
        {
            throw new FileTypeException(new Dictionary<string, string>
            {
                { "File extension", $"Extension {ext} not allowed in App." }
            });
        }

        return fileType;
    }
    
    public static string GetFileTypeFromExtensions(AppFileExt[] extensions)
    {
        var extensionCollection = extensions.Select(e =>
        {
            if (!ExtensionString.TryGetValue(e, out var fileExt))
            {
                throw new FileTypeException(new Dictionary<string, string>
                {
                    { "File extension", $"Extension {fileExt} not allowed in App." }
                });
            }

            return fileExt;
        });

        return string.Join("; ", extensionCollection);
    }
}