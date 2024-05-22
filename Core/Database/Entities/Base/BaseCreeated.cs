namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseCreeated : BaseIdentityKey
{
    public int CreatedById { get; set; }
    public Instant CreatedAt { get; set; }
}