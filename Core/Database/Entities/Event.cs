namespace How.Core.Database.Entities;

using System.ComponentModel.DataAnnotations;
using Base;
using Identity;
using Infrastructure.Enums;
using Storage;

public class Event : BaseLong
{
    [StringLength(1024)]
    public string Name { get; set; }
    public EventStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    
    public int OwnerId { get; set; }
    public HowUser Owner { get; set; }
    
    public ICollection<EventRecord> Records { get; set; }
    
    public int? ImageId { get; set; }
    public Image Image { get; set; }
}