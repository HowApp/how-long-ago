namespace How.Core.Database.Entities.Identity;

using System.ComponentModel.DataAnnotations;
using Storage;

public class HowUser
{
    public int UserId { get; set; }
    [StringLength(2048)]
    public string FirstName { get; set; }
    [StringLength(2048)]
    public string LastName { get; set; }
    
    public bool IsDeleted { get; set; }
    public bool IsSuspended{ get; set; }
    
    public int? StorageImageId { get; set; }
    public StorageImage StorageImage { get; set; }
}