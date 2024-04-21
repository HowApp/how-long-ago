namespace How.Core.Database.Configuration
{
    using How.Core.Database.Entities.Storage;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class AppFileConfiguration : IEntityTypeConfiguration<AppFile>
    {
        public void Configure(EntityTypeBuilder<AppFile> builder)
        {
            builder.HasIndex(x => x.FileHash).IsUnique();
        }
    }
}
