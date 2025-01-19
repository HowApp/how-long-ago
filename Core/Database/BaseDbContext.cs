using How.Core.Database.Entities.Storage;

namespace How.Core.Database;

using Entities.Event;
using Entities.Identity;
using Entities.Record;
using Entities.SharedUser;
using Extensions;
using Microsoft.EntityFrameworkCore;

public class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }

    public DbSet<HowUser> Users { get; set; }
    public DbSet<StorageFile> StorageFiles { get; set; }
    public DbSet<StorageImage> StorageImages { get; set; }

    public DbSet<Event> Events { get; set; } 
    public DbSet<SavedEvent> SavedEvents { get; set; }
    public DbSet<LikedEvent> LikedEvents { get; set; }
    public DbSet<Record> Records { get; set; } 
    public DbSet<LikedRecord> LikedRecords { get; set; }
    public DbSet<RecordImage> RecordImages { get; set; }
    
    public DbSet<SharedUser> SharedUsers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        modelBuilder.SetIdentityName();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        modelBuilder.SetOnDeleteRule();
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}