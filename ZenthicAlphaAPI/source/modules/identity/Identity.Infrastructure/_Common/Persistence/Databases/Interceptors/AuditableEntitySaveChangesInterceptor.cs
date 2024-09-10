using Domain.Entities.Abstractions;
using Identity.Application._Common.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Identity.Infrastructure._Common.Persistence.Databases.Interceptors;

internal class AuditableEntitySaveChangesInterceptor(
    IIdentityService identityService
)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableData(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableData(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableData(DbContext? context)
    {
        if (context is null)
            return;

        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        var currentUserId = currentUserIdentityResult.Match<Guid?>(
            currentUserIdentity => currentUserIdentity.Id,
            none => null,
            failure => null
        );

        var entitiesTracked = context
            .ChangeTracker
            .Entries<IAuditableEntity>();

        foreach (var entry in entitiesTracked)
            UpdateEntityByState(entry, currentUserId);
    }
    private static void UpdateEntityByState(EntityEntry<IAuditableEntity> entry, Guid? currentUserId)
    {
        var state = entry.State;

        if (state is EntityState.Added)
        {
            UpdateAddedData(entry, currentUserId);
            UpdateModifiedData(entry, currentUserId);
        }

        if (state is EntityState.Modified)
            UpdateModifiedData(entry, currentUserId);

        if (state is EntityState.Deleted)
            UpdateDeletedData(entry, currentUserId);
    }
    private static void UpdateAddedData(EntityEntry<IAuditableEntity> entry, Guid? currentUserId)
    {
        var entity = entry.Entity;

        entity.CreatedBy = currentUserId;
        entity.CreationDate = DateTime.UtcNow;
    }
    private static void UpdateModifiedData(EntityEntry<IAuditableEntity> entry, Guid? currentUserId)
    {
        var entity = entry.Entity;

        entity.LastModifiedBy = currentUserId;
        entity.LastModificationDate = DateTime.UtcNow;
    }
    private static void UpdateDeletedData(EntityEntry<IAuditableEntity> entry, Guid? currentUserId)
    {
        var entity = entry.Entity;

        entity.IsDeleted = true;
        entity.DeletedBy = currentUserId;
        entity.DeletionDate = DateTime.UtcNow;

        entry.State = EntityState.Modified;
    }
}