namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseLong : BaseShort
{
    public int? ChangedById { get; set; }
    public Instant? ChangedAt { get; set; }
}