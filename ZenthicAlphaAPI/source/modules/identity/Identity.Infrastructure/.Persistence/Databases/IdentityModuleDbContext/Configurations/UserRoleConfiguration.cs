using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.UsersRoles))
            .ConfigureKeys()
            .HasQueryFilter(entity => !entity.User.IsDeleted && !entity.Role.IsDeleted);
    }
}
