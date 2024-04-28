namespace How.Common.Helpers;

public static class StoragePathHelper
{
    public static class Images
    {
        public static string Image(string name) => $"image/{name}";
        public static string Thumbnail(string name) => $"image/thumbnail/{name}";
    }
    
    public static class Files
    {
        public static string File(string name) => $"file/{name}";
    }
}