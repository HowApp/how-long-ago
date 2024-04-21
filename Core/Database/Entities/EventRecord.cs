namespace How.Core.Database.Entities;

using System.ComponentModel.DataAnnotations;
using Base;
using Storage;

public class EventRecord : BaseShort
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    [StringLength(512)]
    public string Description { get; set; }
    
    // TODO add image of record
    // public int ImageId { get; set; }
    // public AppFile Image { get; set; }
}