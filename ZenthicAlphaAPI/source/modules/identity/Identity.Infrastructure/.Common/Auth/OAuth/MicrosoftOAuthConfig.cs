using Application.Auth;
using Identity.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.Common.Auth.OAuth;

internal static class MicrosoftOAuthConfig
{
    public static void ConfigureOptions(MicrosoftAccountOptions options, AuthSettings authSettings)
    {
        options.ClientId = authSettings.OAuth!.Microsoft!.ClientId;
        options.ClientSecret = authSettings.OAuth!.Microsoft!.ClientSecret;

        options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.UserName), "displayName");
        options.ClaimActions.MapCustomJson(nameof(AuthenticatedSession.Email), user => user.GetString("mail") ?? user.GetString("userPrincipalName"));
    }
}
