namespace How.Server.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowUserClaim : IdentityUserClaim<int>
{
    public virtual HowUser User { get; set; }
}