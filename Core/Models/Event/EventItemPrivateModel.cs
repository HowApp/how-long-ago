namespace How.Core.Models.Event;

using Infrastructure.Enums;

public class EventItemPrivateModel : EventItemPublicModel
{
    public EventStatus Status { get; set; }
    public EventAccessType Access { get; set; }
    public bool IsSavedByUser { get; set; }
    public LikeState OwnLikeState { get; set; }
}