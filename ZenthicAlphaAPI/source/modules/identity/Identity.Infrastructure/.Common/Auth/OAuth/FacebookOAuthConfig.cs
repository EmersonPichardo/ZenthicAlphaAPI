using Identity.Application.Common.Auth;
using Identity.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Identity.Infrastructure.Common.Auth.OAuth;

internal static class FacebookOAuthConfig
{
    public static void ConfigureOptions(FacebookOptions options, AuthSettings authSettings)
    {
        options.ClientId = authSettings.OAuth!.Facebook!.ClientId;
        options.ClientSecret = authSettings.OAuth!.Facebook!.ClientSecret;

        options.ClaimActions.MapJsonKey(nameof(OAuthSession.UserName), "name");
        options.ClaimActions.MapJsonKey(nameof(OAuthSession.Email), "email");
    }
}
