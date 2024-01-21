namespace How.Server.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class HowRoleClaim : IdentityRoleClaim<int>
{
    public virtual HowRole Role { get; set; }
}