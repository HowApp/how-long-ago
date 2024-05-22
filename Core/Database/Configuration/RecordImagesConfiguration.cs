namespace How.Core.Database.Configuration;

using Entities.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecordImagesConfiguration : IEntityTypeConfiguration<RecordImage>
{
    public void Configure(EntityTypeBuilder<RecordImage> builder)
    {
        builder.HasIndex(b => b.MainId).IsUnique();
        builder.HasIndex(b => b.ThumbnailId).IsUnique();
    }
}