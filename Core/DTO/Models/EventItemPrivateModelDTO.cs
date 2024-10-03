namespace How.Core.DTO.Models;

using Infrastructure.Enums;

public class EventItemPrivateModelDTO : EventItemPublicModelDTO
{
    public EventStatus Status { get; set; }
    public EventAccessType Access { get; set; }
}