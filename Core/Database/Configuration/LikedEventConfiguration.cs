namespace How.Core.Database.Configuration;

using Entities.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LikedEventConfiguration : IEntityTypeConfiguration<LikedEvent>
{
    public void Configure(EntityTypeBuilder<LikedEvent> builder)
    {
        builder.HasKey(k => new {k.EventId, k.LikedByUserId});
        builder.HasIndex(i => new {i.EventId, i.LikedByUserId}).IsUnique();
    }
}