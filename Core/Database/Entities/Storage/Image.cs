namespace How.Core.Database.Entities.Storage;

using Base;

public class Image : BaseIdentityKey
{
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    
    public int MainId { get; set; }
    public FileStorage Main { get; set; }
    
    public int ThumbnailId { get; set; }
    public FileStorage Thumbnail { get; set; }
}