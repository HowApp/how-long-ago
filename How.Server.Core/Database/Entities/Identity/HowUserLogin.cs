namespace How.Server.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowUserLogin : IdentityUserLogin<int>
{
    public virtual HowUser User { get; set; }
}