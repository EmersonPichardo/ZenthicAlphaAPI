using Application._Common.Persistence.Databases;
using Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;

internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder
            .ToTable(nameof(IApplicationDbContext.RolesPermissions))
            .ConfigureKeys()
            .ConfigureDeletionFilter();
    }
}
