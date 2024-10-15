namespace How.Core.DTO.Models;

using NodaTime;

public class RecordItemPublicModelDTO
{
    public int Id { get; set; }
    public string Description { get; set; }
    public Instant CreatedAt { get; set; }
    public ICollection<ImageModelDTO> Images { get; set; } = new List<ImageModelDTO>();
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}