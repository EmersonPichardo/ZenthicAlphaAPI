using Application._Common.Persistence.Databases;
using Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder
            .ToTable(nameof(IApplicationDbContext.UsersRoles))
            .ConfigureKeys()
            .ConfigureDeletionFilter();
    }
}
