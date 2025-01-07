namespace How.Core.Database;

using Extensions;
using Microsoft.EntityFrameworkCore;
using TemporaryStorageEntity;

public class TemporaryStorageDbContext : DbContext
{
    public TemporaryStorageDbContext(DbContextOptions<TemporaryStorageDbContext> options) : base(options)
    {
    }

    public DbSet<File> Files { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("temporary");
        
        modelBuilder.SetOnDeleteRule();
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}