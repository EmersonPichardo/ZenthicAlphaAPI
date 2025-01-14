﻿using Application.Auth;
using Domain.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Databases;

public class AuditableEntitySaveChangesInterceptor(
    IUserSessionService userSessionService
)
    : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await UpdateAuditableDataAsync(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task UpdateAuditableDataAsync(DbContext? context)
    {
        if (context is null)
            return;

        var userSession = await userSessionService.GetSessionAsync();

        Guid? currentUserId = userSession is AuthenticatedSession authenticatedSession
            ? authenticatedSession.Id
            : null;

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