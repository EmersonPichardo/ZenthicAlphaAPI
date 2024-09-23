using Identity.Application._Common.Persistence.Databases;
using Identity.Infrastructure._Common.Persistence.Databases.Interceptors;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext;

internal class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
)
    : DbContext(options)
    , IIdentityDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("identity")
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .MapToNormalizeMethod();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .AddInterceptors(auditableEntitySaveChangesInterceptor)
            .UseEnumCheckConstraints();
}
