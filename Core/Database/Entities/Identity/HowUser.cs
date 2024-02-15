namespace How.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowUser : IdentityUser<int>
{
    public virtual ICollection<HowUserRole> UserRoles {get; set;}
}