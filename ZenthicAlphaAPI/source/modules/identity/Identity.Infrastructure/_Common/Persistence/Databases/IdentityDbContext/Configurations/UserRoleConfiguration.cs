using Identity.Application._Common.Persistence.Databases;
using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder
            .ToTable(nameof(IIdentityDbContext.UsersRoles))
            .ConfigureKeys()
            .ConfigureDeletionFilter();
    }
}
