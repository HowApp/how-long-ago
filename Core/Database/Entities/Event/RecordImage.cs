namespace How.Core.Database.Entities.Event;

using Base;
using Storage;

public class RecordImage : BaseIdentityKey
{
    public int RecordId { get; set; }
    public Record Record { get; set; }
    
    public int Position { get; set; }
    
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    
    public int ThumbnailHeight { get; set; }
    public int ThumbnailWidth { get; set; }
    
    public int MainId { get; set; }
    public StorageFile Main { get; set; }
    
    public int ThumbnailId { get; set; }
    public StorageFile Thumbnail { get; set; }
}