namespace How.Core.Database.Configuration;

using Entities.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SavedEventConfiguration : IEntityTypeConfiguration<SavedEvent>
{
    public void Configure(EntityTypeBuilder<SavedEvent> builder)
    {
        builder.HasKey(k => new { k.EventId, k.UserId });
        builder.HasIndex(i => new { i.EventId, i.UserId }).IsUnique();
    }
}