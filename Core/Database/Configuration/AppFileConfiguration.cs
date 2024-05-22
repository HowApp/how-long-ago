namespace How.Core.Database.Configuration;

using Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppFileConfiguration : IEntityTypeConfiguration<StorageFile>
{
    public void Configure(EntityTypeBuilder<StorageFile> builder)
    {
        builder.HasIndex(x => x.Hash).IsUnique();
        builder.HasIndex(x => x.Path).IsUnique();
        
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Path).IsRequired();
    }
}
