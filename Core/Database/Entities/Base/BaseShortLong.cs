namespace How.Core.Database.Entities.Base;

public class BaseShortLong : BaseShort
{
    public int? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
}