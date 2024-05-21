namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseShort : BaseIdentityKey
{
    public int CreatedById { get; set; }
    public Instant CreatedAt { get; set; }
}