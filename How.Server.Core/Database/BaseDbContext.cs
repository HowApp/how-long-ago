namespace How.Server.Core.Database;

using Microsoft.EntityFrameworkCore;

public class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
    }
}