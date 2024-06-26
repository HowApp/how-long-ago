namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseChanged : BaseCreated
{
    public int? ChangedById { get; set; }
    public Instant? ChangedAt { get; set; }
}