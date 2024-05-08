namespace How.Core.Database.Entities.Base;

using NodaTime;

public class BaseLong : BaseShort
{
    public int? ChangedBy { get; set; }
    public Instant? ChangedAt { get; set; }
}