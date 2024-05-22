namespace How.Core.Database.Configuration;

using Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StorageImageConfiguration : IEntityTypeConfiguration<StorageImage>
{
    public void Configure(EntityTypeBuilder<StorageImage> builder)
    {
        builder.HasIndex(b => new { b.MainId }).IsUnique();
        builder.HasIndex(b => new { b.ThumbnailId }).IsUnique();
    }
}