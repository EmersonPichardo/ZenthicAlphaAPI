using Application.Persistence.Databases;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application._Common.Persistence.Databases;

public interface IIdentityDbContext : IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolesPermissions => Set<RolePermission>();
    public DbSet<UserRole> UsersRoles => Set<UserRole>();
}
