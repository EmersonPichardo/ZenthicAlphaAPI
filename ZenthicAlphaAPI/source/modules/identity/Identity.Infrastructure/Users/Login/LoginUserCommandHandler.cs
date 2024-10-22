using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users.Login;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;

namespace Identity.Infrastructure.Users.Login;

internal class LoginUserCommandHandler(
    IdentityModuleDbContext dbContext,
    PasswordManager passwordManager,
    IOptions<AuthSettings> authSettingsOptions,
    JwtManager jwtManager
)
    : IRequestHandler<LoginUserCommand, OneOf<LoginUserCommandResponse, Failure>>
{
    private readonly AuthSettings.JwtSettings jwtSettings = authSettingsOptions.Value.Jwt;

    public async Task<OneOf<LoginUserCommandResponse, Failure>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity.Permissions)
            .SingleOrDefaultAsync(
                user => user.Email.Equals(command.Email),
                cancellationToken
            );

        var isInvalidUser = foundUser is null
            || foundUser.Status.HasFlag(UserStatus.Inactive)
            || !passwordManager.Equals(command.Password, foundUser.HashedPassword, foundUser.HashingStamp);

        if (foundUser is null || isInvalidUser)
            return FailureFactory.UnauthorizedAccess(detail: "Email incorrecto o contraseña incorrecta");

        var userAccess = foundUser
            .UserRoles
            .Select(entity => entity.Role)
            .SelectMany(entity => entity.Permissions)
            .Where(entity => entity.RequiredAccess > 0)
            .GroupBy(
                entity => entity.Component,
                entity => entity.RequiredAccess,
                (Component, RequiredAccesses) => new { Component, RequiredAccesses }
            )
            .ToDictionary(
                entity => entity.Component.ToString(),
                entity => entity.RequiredAccesses.Aggregate((accessLevel, rolePermission) => accessLevel.AddFlag(rolePermission))
            );

        var response = new LoginUserCommandResponse
        {
            DisplayName = foundUser.UserName,
            Status = foundUser.Status.ToString(),
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
                Value = jwtManager.GenerateRefreshToken(foundUser.Id)
            },
            Token = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
                Value = jwtManager.Generate(foundUser, userAccess)
            },
            Access = userAccess.ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.AsString()
            )
        };

        return response;
    }
}
