using Application._Common.Caching;
using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Presentation._Common.Security;

internal record CurrentUserService(
    ICacheStore cacheStore,
    IHttpContextAccessor httpContextAccessor,
    IIdentityService identityService,
    IApplicationDbContext dbContext
)
    : ICurrentUserService
{
    public async Task<OneOf<ICurrentUser, None, Failure>> GetCurrentUserAsync()
    {
        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        if (currentUserIdentityResult.IsT1)
            return new None();

        if (currentUserIdentityResult.IsT2)
            return currentUserIdentityResult.AsT2;

        var currentUserId = currentUserIdentityResult.AsT0.Id;

        var cancellationToken = httpContextAccessor
            .HttpContext?
            .RequestAborted
        ?? CancellationToken.None;

        var cachedCurrentUser = await cacheStore.GetAsync<CurrentUser>(
            $"{nameof(ICurrentUserIdentity)}{{{currentUserId}}}",
            cancellationToken
        );

        if (cachedCurrentUser is not null)
            return cachedCurrentUser;

        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity != null ? entity.Permissions : null)
            .FirstOrDefaultAsync(
                entity => entity.Id.Equals(currentUserId),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.NotFound("User not found", $"No user was found with an Id of {currentUserId}");

        var accesses = foundUser
            .UserRoles
            .Select(entity => entity.Role)
            .SelectMany(entity => entity?.Permissions ?? [])
            .GroupBy(
                entity => entity.Component,
                entity => entity.RequiredAccess
            )
            .ToDictionary(
                group => group.Key,
                group => group.Aggregate(
                    (requiredAccess, currentAccess) => requiredAccess | currentAccess
                )
            );

        var currentUser = new CurrentUser()
        {
            Id = currentUserId,
            Status = foundUser.Status,
            Accesses = accesses
        };

        await cacheStore.SetAsync(
            nameof(ICurrentUserIdentity),
            $"{nameof(ICurrentUserIdentity)}{{{currentUserId}}}",
            currentUser,
            cancellationToken
        );

        return currentUser;
    }
}