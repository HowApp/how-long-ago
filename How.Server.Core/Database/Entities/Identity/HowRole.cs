namespace How.Server.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowRole : IdentityRole<int>
{
    public virtual ICollection<HowUserRole> UserRoles { get; set; }
    public virtual ICollection<HowRoleClaim> RoleClaims { get; set; }
}