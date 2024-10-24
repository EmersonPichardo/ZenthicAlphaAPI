using Application.Persistence.Databases;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext;

internal class IdentityModuleDbContext(
    DbContextOptions<IdentityModuleDbContext> options,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
)
    : DbContext(options)
    , IApplicationDbContext
{
    internal DbSet<User> Users => Set<User>();
    internal DbSet<OAuthUser> OAuthUsers => Set<OAuthUser>();
    internal DbSet<UserToken> UserTokens => Set<UserToken>();
    internal DbSet<Role> Roles => Set<Role>();
    internal DbSet<RolePermission> RolesPermissions => Set<RolePermission>();
    internal DbSet<UserRole> UsersRoles => Set<UserRole>();
    internal DbSet<OAuthUserRole> OAuthUsersRoles => Set<OAuthUserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("identity")
            .ApplyConfigurationsFromAssembly(AssemblyReference.Assembly)
            .MapToNormalizeMethod();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(auditableEntitySaveChangesInterceptor)
            .UseEnumCheckConstraints();
}
