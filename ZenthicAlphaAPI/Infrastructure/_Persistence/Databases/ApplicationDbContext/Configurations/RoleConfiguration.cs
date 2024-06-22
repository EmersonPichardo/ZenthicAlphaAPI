using Application._Common.Persistence.Databases;
using Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    internal static readonly int NameLength = 64;

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .ToTable(nameof(IApplicationDbContext.Roles))
            .ConfigureKeys()
            .ConfigureDeletionFilter();

        builder.HasIndex(entity => entity.Name).ConfigureUniqueness();

        builder.Property(entity => entity.Name).HasMaxLength(NameLength);
    }
}
