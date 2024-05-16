namespace How.Core.Models.Event;

public class EventItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string EventMainHash { get; set; }
    public string EventThumbnailHash { get; set; }
    public int OwnerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OwnerMainHash { get; set; }
    public string OwnerThumbnailHash { get; set; }
}