namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseCreated : IdentityKey
{
    public int CreatedById { get; set; }
    public Instant CreatedAt { get; set; }
}