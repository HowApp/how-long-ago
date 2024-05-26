namespace How.Core.Database.Entities.Event;

using Base;
using Storage;

public class RecordImage : BaseIdentityKey
{
    public int RecordId { get; set; }
    public Record Record { get; set; }
    
    public int Position { get; set; }
    
    public int ImageId { get; set; }
    public StorageImage Image { get; set; }
}