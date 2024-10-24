using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Modularity;
using Identity.Application.Auth;
using Identity.Application.Auth.AddOAuthUser;
using Identity.Application.Auth.OAuthCallback;
using Identity.Application.Auth.UpdateOAuthUser;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;
using System.Text;
using System.Text.Json;

namespace Identity.Infrastructure.Auth.OAuthCallback;

internal class OAuthCallbackCommandHandler(
    IUserSessionService userSessionService,
    IdentityModuleDbContext dbContext,
    ISender sender,
    IOptions<AuthSettings> authSettingsOptions,
    JwtManager jwtManager
)
    : IRequestHandler<OAuthCallbackCommand, OneOf<string, Failure>>
{
    private readonly AuthSettings.JwtSettings jwtSettings = authSettingsOptions.Value.Jwt;

    public async Task<OneOf<string, Failure>> Handle(OAuthCallbackCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionService.GetSessionAsync();
        var oauthSession = (OAuthSession)userSession;

        var oauthUserResult = await AddOrUpdateOAuthUserAsync(oauthSession, cancellationToken);
        if (oauthUserResult.IsFailure()) return oauthUserResult.GetFailure();
        var (oauthUser, userAccesses) = oauthUserResult.GetValue<(OAuthUserDto, IReadOnlyDictionary<string, Permission>)>();

        var jwtRequest = new JwtManager.JwtRequest
        {
            Id = oauthUser.Id.ToString(),
            UserName = oauthSession.UserName,
            Email = oauthSession.Email,
            Status = oauthSession.Status.ToString(),
            Accesses = userAccesses
        };

        var loginResponse = new LoginResponse
        {
            UserName = oauthSession.UserName,
            Statuses = oauthSession.Status.AsString(),
            Accesses = userAccesses.ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.AsString()
            ),
            AccessToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
                Value = jwtManager.Generate(jwtRequest)
            },
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
                Value = jwtManager.GenerateRefreshToken()
            }
        };

        var jsonResponse = JsonSerializer.Serialize(loginResponse);
        var jsonResponseBytes = Encoding.Default.GetBytes(jsonResponse);
        var base64Response = Convert.ToBase64String(jsonResponseBytes);

        return $"{oauthSession.RedirectUrl}?base64-response={base64Response}";
    }

    private async Task<OneOf<(OAuthUserDto, IReadOnlyDictionary<string, Permission>), Failure>> AddOrUpdateOAuthUserAsync(
        OAuthSession oauthSession, CancellationToken cancellationToken)
    {
        var foundOAuthUser = await dbContext.OAuthUsers
            .AsNoTrackingWithIdentityResolution()
            .Include(oauthUser => oauthUser.OAuthUserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity.Permissions)
            .FirstOrDefaultAsync(
                oauthUser => oauthUser.Email == oauthSession.Email,
                cancellationToken
            );

        if (foundOAuthUser is null)
        {
            var addOAuthUserCommand = new AddOAuthUserCommand
            {
                UserName = oauthSession.UserName,
                Email = oauthSession.Email
            };

            var addOAuthUserCommandResult = await sender.Send(addOAuthUserCommand, cancellationToken);
            if (addOAuthUserCommandResult.IsFailure()) return addOAuthUserCommandResult.GetFailure();

            return (
                addOAuthUserCommandResult.GetValue<AddOAuthUserCommandResponse>(),
                new Dictionary<string, Permission>()
            );
        }
        else
        {
            if (foundOAuthUser.Status.HasFlag(OAuthUserStatus.Inactive))
                return FailureFactory.UnauthorizedAccess();

            var updateOAuthUserCommand = new UpdateOAuthUserCommand
            {
                Id = foundOAuthUser.Id,
                UserName = oauthSession.UserName,
                Email = oauthSession.Email
            };

            var updateOAuthUserCommandResult = await sender.Send(updateOAuthUserCommand, cancellationToken);
            if (updateOAuthUserCommandResult.IsFailure()) return updateOAuthUserCommandResult.GetFailure();

            var userAccesses = foundOAuthUser
                .OAuthUserRoles
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

            return (
                OAuthUserDto.FromOAuthUser(foundOAuthUser),
                userAccesses
            );
        }
    }
}