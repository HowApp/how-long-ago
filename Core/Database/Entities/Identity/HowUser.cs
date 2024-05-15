namespace How.Core.Database.Entities.Identity;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Storage;

public class HowUser : IdentityUser<int>
{
    [StringLength(2048)]
    public string FirstName { get; set; }
    [StringLength(2048)]
    public string LastName { get; set; }
    
    public int? StorageImageId { get; set; }
    public StorageImage StorageImage { get; set; }
    public virtual ICollection<HowUserRole> UserRoles {get; set;}
}