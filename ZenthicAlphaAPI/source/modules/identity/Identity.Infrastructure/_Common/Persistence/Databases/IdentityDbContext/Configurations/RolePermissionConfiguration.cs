using Identity.Application._Common.Persistence.Databases;
using Identity.Domain.Roles;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;

internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder
            .ToTable(nameof(IIdentityDbContext.RolesPermissions))
            .ConfigureKeys()
            .ConfigureDeletionFilter();
    }
}
