namespace How.Core.Database.Configuration;

using Entities.Record;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LikedRecordConfiguration : IEntityTypeConfiguration<LikedRecord>
{
    public void Configure(EntityTypeBuilder<LikedRecord> builder)
    {
        builder.HasIndex(i => new {i.RecordId, i.LikedByUserId}).IsUnique();
    }
}