namespace How.Core.Database.Configuration;

using Entities.Record;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecordImageConfiguration : IEntityTypeConfiguration<RecordImage>
{
    public void Configure(EntityTypeBuilder<RecordImage> builder)
    {
        builder.HasIndex(b => new {b.RecordId, b.ImageId}).IsUnique();
        builder.HasIndex(b => b.ImageId).IsUnique();
    }
}