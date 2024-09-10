using Identity.Application._Common.Persistence.Databases;
using Identity.Domain.Roles;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    internal static readonly int NameLength = 64;

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .ToTable(nameof(IIdentityDbContext.Roles))
            .ConfigureKeys()
            .ConfigureDeletionFilter();

        builder.HasIndex(entity => entity.Name).ConfigureUniqueness();

        builder.Property(entity => entity.Name).HasMaxLength(NameLength);
    }
}
