namespace How.Core.DTO.Models;

using NodaTime;

public class EventItemModelDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ImageModelDTO Image { get; set; }
    public OwnerModelDTO Owner { get; set; }
    public Instant CreatedAt { get; set; }
}