using Domain.Modularity;
using Identity.Domain.Roles;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.RolesPermissions))
            .ConfigureKeys();

        builder.Property(entity => entity.Component).HasColumnType("varchar(50)").HasConversion<EnumToStringConverter<Component>>();
        builder.Property(entity => entity.RequiredAccess).HasColumnType("varchar(50)").HasConversion<EnumToStringConverter<Permission>>();
    }
}
