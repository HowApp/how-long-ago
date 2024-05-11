namespace How.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;
using Storage;

public class HowUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public int? StorageImageId { get; set; }
    public StorageImage StorageImage { get; set; }
    public virtual ICollection<HowUserRole> UserRoles {get; set;}
}