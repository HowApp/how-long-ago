namespace How.Core.Database.Entities.Event;

using System.ComponentModel.DataAnnotations;
using Base;
using Identity;
using Infrastructure.Enums;
using Storage;

public class Event : BaseChanged
{
    [StringLength(1024)]
    public string Name { get; set; }
    public EventStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    
    public int OwnerId { get; set; }
    public HowUser Owner { get; set; }
    
    public ICollection<Record> Records { get; set; }
    
    public int? StorageImageId { get; set; }
    public StorageImage StorageImage { get; set; }
}