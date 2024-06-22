using Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application._Common.Persistence.Databases;

public interface IApplicationDbContext
{
    //dbo
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolesPermissions => Set<RolePermission>();
    public DbSet<UserRole> UsersRoles => Set<UserRole>();

    //base
    public DatabaseFacade Database { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
