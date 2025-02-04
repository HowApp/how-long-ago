namespace How.Core.Models.Event;

using NodaTime;

public class EventItemPublicModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Instant CreatedAt { get; set; }
    public string EventMainHash { get; set; }
    public string EventThumbnailHash { get; set; }
    public int OwnerId { get; set; }
    public string OwnerFirstName { get; set; }
    public string OwnerLastName { get; set; }
    public string OwnerMainHash { get; set; }
    public string OwnerThumbnailHash { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int SavedCount { get; set; }
}