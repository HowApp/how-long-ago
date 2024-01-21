namespace How.Server.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowUser : IdentityUser<int>
{
    public virtual ICollection<HowUserClaim> Claims {get; set;}
    public virtual ICollection<HowUserLogin> Logins {get; set;}
    public virtual ICollection<HowUserToken> Tokens {get; set;}
    public virtual ICollection<HowUserRole> UserRoles {get; set;}
}