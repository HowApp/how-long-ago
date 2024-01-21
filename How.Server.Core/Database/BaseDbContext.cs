namespace How.Server.Core.Database;

using Entities.Identity;
using Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BaseDbContext : IdentityDbContext<
    HowUser, 
    HowRole, 
    int, 
    HowUserClaim, 
    HowUserRole, 
    HowUserLogin, 
    HowRoleClaim, 
    HowUserToken>
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.SetIdentityName();
        modelBuilder.SetIdentityRule();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        modelBuilder.SetOnDeleteRule();
    }
}