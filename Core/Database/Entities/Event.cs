namespace How.Core.Database.Entities;

using System.ComponentModel.DataAnnotations;
using Base;
using Storage;

public class Event : BaseShortLong
{
    [StringLength(512)]
    public string Name { get; set; }
    public bool IsActive { get; set; }
    
    public ICollection<EventRecord> Records { get; set; }
    
    // TODO add main image of event
    // public int ImageId { get; set; }
    // public AppFile Image { get; set; }
}