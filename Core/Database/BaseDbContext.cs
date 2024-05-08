using How.Core.Database.Entities.Storage;

namespace How.Core.Database;

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

    public DbSet<FileStorage> AppFiles { get; set; }
    public DbSet<Image> StorageImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.SetIdentityName();
        modelBuilder.SetIdentityRule();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        modelBuilder.SetOnDeleteRule();
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}