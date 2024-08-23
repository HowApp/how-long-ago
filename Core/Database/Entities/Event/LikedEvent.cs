namespace How.Core.Database.Entities.Event;

using Identity;
using Infrastructure.Enums;

public class LikedEvent
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    public int LikedByUserId { get; set; }
    public HowUser LikedByUser { get; set; }
    
    public LikeState State { get; set; }
}