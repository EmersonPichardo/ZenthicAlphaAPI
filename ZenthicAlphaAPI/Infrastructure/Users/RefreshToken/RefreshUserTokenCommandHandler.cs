using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application.Users.ClearSession;
using Application.Users.RefreshToken;
using Domain.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Infrastructure.Users.RefreshToken;

internal class RefreshUserTokenCommandHandler(
    IIdentityService identityService,
    IApplicationDbContext dbContext,
    IJwtService jwtService,
    ISender mediator
)
    : IRefreshUserTokenCommandHandler
{
    public async Task<OneOf<RefreshUserTokenCommandResponse, Failure>> Handle(RefreshUserTokenCommand command, CancellationToken cancellationToken)
    {
        if (identityService.IsNotRefreshTokenCaller())
            throw new UnauthorizedAccessException();

        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        if (currentUserIdentityResult.IsT1)
            return FailureFactory.UnauthorizedAccess();

        if (currentUserIdentityResult.IsT2)
            return currentUserIdentityResult.AsT2;

        var currentUserId = currentUserIdentityResult.AsT0.Id;

        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity != null ? entity.Permissions : null)
            .SingleOrDefaultAsync(
                user
                    => user.Id.Equals(currentUserId)
                    && !user.Status.Equals(UserStatus.Inactive),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.UnauthorizedAccess();

        var userAccess = foundUser
            .UserRoles
            .Select(entity => entity.Role)
            .SelectMany(entity => entity?.Permissions ?? [])
            .Where(entity
                => entity.RequiredAccess > 0
            )
            .GroupBy(
                entity => entity.Component,
                entity => entity.RequiredAccess
            )
            .ToDictionary(
                entity => entity.Key.ToString(),
                entity => entity.Aggregate((accessLevel, rolePermission) => accessLevel | rolePermission)
            );

        var response = new RefreshUserTokenCommandResponse()
        {
            DisplayName = foundUser.FullName,
            Status = foundUser.Status.ToString(),
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtService.GetSettings().RefreshTokenLifetime),
                Value = jwtService.GenerateJwtRefreshToken(foundUser.Id)
            },
            Token = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtService.GetSettings().TokenLifetime),
                Value = jwtService.GenerateJwtToken(foundUser.Id)
            },
            Access = userAccess
        };

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = foundUser.Id },
            cancellationToken
        );

        return response;
    }
}
