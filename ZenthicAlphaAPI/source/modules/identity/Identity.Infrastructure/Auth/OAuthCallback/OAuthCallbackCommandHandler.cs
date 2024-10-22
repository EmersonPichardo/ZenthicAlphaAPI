using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Identity.Application.OAuth.OAuthCallback;
using Identity.Application.Users.Login;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OneOf;
using System.Text.Json;

namespace Identity.Infrastructure.Auth.OAuthCallback;

internal class OAuthCallbackCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthSettings> authSettingsOptions,
    JwtManager jwtManager
)
    : IRequestHandler<OAuthCallbackCommand, OneOf<string, Failure>>
{
    private readonly AuthSettings.JwtSettings jwtSettings = authSettingsOptions.Value.Jwt;

    public async Task<OneOf<string, Failure>> Handle(OAuthCallbackCommand command, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
            return FailureFactory.UnauthorizedAccess();

        var oauthUserResult = await GetOAuthUserAsync(httpContext, command.AuthenticationScheme);
        if (oauthUserResult.IsFailure()) return oauthUserResult.GetFailure();
        var (redirectUrl, oauthUser) = oauthUserResult.GetValue<(string, OAuthUser)>();

        var loginResponse = new LoginUserCommandResponse
        {
            DisplayName = oauthUser.UserName,
            Status = oauthUser.Status.ToString(),
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime),
                Value = jwtManager.GenerateRefreshToken(oauthUser.Id)
            },
            Token = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtSettings.TokenLifetime),
                Value = jwtManager.Generate(oauthUser)
            },
            Access = new Dictionary<string, string[]>()
        };

        var jsonResponse = JsonSerializer.Serialize(loginResponse);
        var jsonResponseBytes = EncodingHelper.GetBytes(jsonResponse);
        var base64Response = Convert.ToBase64String(jsonResponseBytes);

        return $"{redirectUrl}?token={base64Response}";
    }

    private static async Task<OneOf<(string redirectUrl, OAuthUser oauthUser), Failure>> GetOAuthUserAsync(HttpContext httpContext, string authenticationScheme)
    {
        var authenticationResult = await httpContext.AuthenticateAsync(authenticationScheme);

        if (!authenticationResult.Succeeded)
            return FailureFactory.UnauthorizedAccess();

        var userNameClaimResult = authenticationResult.Principal.GetStringByName(nameof(AuthenticatedSession.UserName));
        if (userNameClaimResult.IsFailure()) return FailureFactory.UnauthorizedAccess();
        var userName = userNameClaimResult.GetValue<string>();

        var emailClaimResult = authenticationResult.Principal.GetStringByName(nameof(AuthenticatedSession.Email));
        if (emailClaimResult.IsFailure()) return FailureFactory.UnauthorizedAccess();
        var email = emailClaimResult.GetValue<string>();

        var statusClaimResult = authenticationResult.Principal.GetEnumByName<OAuthUserStatus>(nameof(AuthenticatedSession.Status));
        if (statusClaimResult.IsFailure()) return FailureFactory.UnauthorizedAccess();
        var status = statusClaimResult.GetValue<OAuthUserStatus>();

        var redirectUrl = authenticationResult.Properties.Items["redirectUrl"];

        if (redirectUrl is null)
            return FailureFactory.UnauthorizedAccess();

        return (
            redirectUrl,
            new OAuthUser
            {
                UserName = userName,
                Email = email,
                Status = status
            }
        );
    }
}
