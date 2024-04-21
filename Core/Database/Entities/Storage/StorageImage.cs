namespace How.Core.Database.Entities.Storage;

using Base;

public class StorageImage : BaseIdentityKey
{
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    
    public int ImageId { get; set; }
    public AppFile Image { get; set; }
    
    public int ThumbnailId { get; set; }
    public AppFile Thumbnail { get; set; }
}