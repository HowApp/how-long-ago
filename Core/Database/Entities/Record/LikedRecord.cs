namespace How.Core.Database.Entities.Record;

using Identity;
using Infrastructure.Enums;

public class LikedRecord
{
    public int RecordId { get; set; }
    public Record Record { get; set; }
    
    public int LikedByUserId { get; set; }
    public HowUser LikedByUser { get; set; }
    
    public LikeState State { get; set; }
}