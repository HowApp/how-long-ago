namespace How.Core.Database.Configuration;

using Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppFileConfiguration : IEntityTypeConfiguration<StorageFile>
{
    public void Configure(EntityTypeBuilder<StorageFile> builder)
    {
        builder.HasIndex(x => x.Hash).IsUnique();
    }
}
