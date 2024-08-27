namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseCreated : PKey
{
    public int CreatedById { get; set; }
    public Instant CreatedAt { get; set; }
}