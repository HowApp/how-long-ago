namespace How.Core.Database.Entities;

using System.ComponentModel.DataAnnotations;
using Base;
using Storage;

public class EventRecord : BaseShort
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    [StringLength(2048)]
    public string Description { get; set; }
    
    public int? ImageId { get; set; }
    public Image Image { get; set; }
}