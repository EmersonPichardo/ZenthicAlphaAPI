using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users.RefreshToken;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;

namespace Identity.Infrastructure.Users.RefreshToken;

internal class RefreshUserTokenCommandHandler(
    IUserSessionInfo userSessionInfo,
    IdentityModuleDbContext dbContext,
    IOptions<AuthSettings> authSettingsOptions,
    JwtManager jwtManager
)
    : IRequestHandler<RefreshUserTokenCommand, OneOf<RefreshUserTokenCommandResponse, Failure>>
{
    private readonly IUserSession userSession = userSessionInfo.Session;
    private readonly AuthSettings.JwtSettings jwtSettings = authSettingsOptions.Value.Jwt;

    public async Task<OneOf<RefreshUserTokenCommandResponse, Failure>> Handle(RefreshUserTokenCommand command, CancellationToken cancellationToken)
    {
        if (userSession is not RefreshTokenSession refreshTokenSession)
            return FailureFactory.UnauthorizedAccess();

        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity.Permissions)
            .SingleOrDefaultAsync(
                user => user.Id.Equals(refreshTokenSession.UserId),
                cancellationToken
            );

        if (foundUser is null || foundUser.Status.HasFlag(UserStatus.Inactive))
            return FailureFactory.UnauthorizedAccess();

        var userAccess = foundUser
            .UserRoles
            .Select(entity => entity.Role)
            .SelectMany(entity => entity.Permissions)
            .Where(entity => entity.RequiredAccess > 0)
            .GroupBy(
                entity => entity.Component,
                entity => entity.RequiredAccess
            )
            .ToDictionary(
                entity => entity.Key.ToString(),
                entity => entity.Aggregate((accessLevel, rolePermission) => accessLevel | rolePermission)
            );

        var response = new RefreshUserTokenCommandResponse
        {
            DisplayName = foundUser.UserName,
            Status = foundUser.Status.ToString(),
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
                Value = jwtManager.GenerateJwtRefreshToken(foundUser.Id)
            },
            Token = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
                Value = jwtManager.GenerateJwtToken(foundUser, userAccess)
            },
            Access = userAccess.ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.AsString()
            )
        };

        return response;
    }
}
