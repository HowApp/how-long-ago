namespace How.Core.Database.Entities.Event;

using Identity;

public class SavedEvent
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    public int UserId { get; set; }
    public HowUser User { get; set; }
}