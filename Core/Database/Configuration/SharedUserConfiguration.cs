namespace How.Core.Database.Configuration;

using Entities.SharedUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SharedUserConfiguration : IEntityTypeConfiguration<SharedUser>
{
    public void Configure(EntityTypeBuilder<SharedUser> builder)
    {
        builder.HasIndex(x => new { x.UserOwnerId, x.UserSharedId }).IsUnique();
    }
}