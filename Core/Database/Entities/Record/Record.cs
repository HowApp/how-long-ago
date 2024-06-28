namespace How.Core.Database.Entities.Record;

using System.ComponentModel.DataAnnotations;
using Base;
using Event;

public class Record : BaseCreated
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    [StringLength(2048)]
    public string Description { get; set; }
    
    public ICollection<RecordImage> RecordImages { get; set; }
}