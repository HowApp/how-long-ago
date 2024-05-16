using How.Core.Database.Entities.Storage;

namespace How.Core.Database;

using Entities.Event;
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

    public DbSet<StorageFile> StorageFiles { get; set; }
    public DbSet<StorageImage> StorageImages { get; set; }

    public DbSet<Event> Events { get; set; } 
    public DbSet<EventRecord> EventRecords { get; set; } 
    
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