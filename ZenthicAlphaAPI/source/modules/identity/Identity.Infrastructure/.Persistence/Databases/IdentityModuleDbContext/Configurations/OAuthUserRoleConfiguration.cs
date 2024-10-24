using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class OAuthUserRoleConfiguration : IEntityTypeConfiguration<OAuthUserRole>
{
    public void Configure(EntityTypeBuilder<OAuthUserRole> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.OAuthUsersRoles))
            .ConfigureKeys()
            .HasQueryFilter(entity => !entity.Role.IsDeleted);
    }
}
