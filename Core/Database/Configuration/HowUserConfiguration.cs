namespace How.Core.Database.Configuration;

using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HowUserConfiguration : IEntityTypeConfiguration<HowUser>
{
    public void Configure(EntityTypeBuilder<HowUser> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}