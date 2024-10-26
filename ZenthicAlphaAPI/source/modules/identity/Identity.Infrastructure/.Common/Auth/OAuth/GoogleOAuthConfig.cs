using Application.Auth;
using Identity.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace Identity.Infrastructure.Common.Auth.OAuth;

internal static class GoogleOAuthConfig
{
    public static void ConfigureOptions(GoogleOptions options, AuthSettings authSettings)
    {
        options.ClientId = authSettings.OAuth!.Google!.ClientId;
        options.ClientSecret = authSettings.OAuth!.Google!.ClientSecret;

        options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.UserName), "name");
        options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.Email), "email");
    }
}
