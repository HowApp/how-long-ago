namespace How.Core.DTO.Models;

using Infrastructure.Enums;
using NodaTime;

public class EventItemModelDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EventAccessType Access { get; set; }
    public ImageModelDTO Image { get; set; }
    public UserInfoModelShortDTO UserInfo { get; set; }
    public Instant CreatedAt { get; set; }
}