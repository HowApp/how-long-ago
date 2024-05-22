namespace How.Core.Database.Entities.Event;

using System.ComponentModel.DataAnnotations;
using Base;

public class Record : BaseCreeated
{
    public int EventId { get; set; }
    public Event Event { get; set; }
    
    [StringLength(2048)]
    public string Description { get; set; }
    
    public ICollection<RecordImage> RecordImages { get; set; }
}