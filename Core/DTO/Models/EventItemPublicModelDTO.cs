namespace How.Core.DTO.Models;

using NodaTime;

public class EventItemPublicModelDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ImageModelDTO Image { get; set; }
    public UserInfoModelShortDTO UserInfo { get; set; }
    public Instant CreatedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int SavedCount { get; set; }
}