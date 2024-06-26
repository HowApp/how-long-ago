namespace How.Core.Database.Entities.Storage;

using Base;

public class StorageImage : IdentityKey
{
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    
    public int MainId { get; set; }
    public StorageFile Main { get; set; }
    
    public int ThumbnailId { get; set; }
    public StorageFile Thumbnail { get; set; }
}