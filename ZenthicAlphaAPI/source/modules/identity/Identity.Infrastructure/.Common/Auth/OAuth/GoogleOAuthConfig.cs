using Application.Auth;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace Identity.Infrastructure.Common.Auth.OAuth;

internal static class GoogleOAuthConfig
{
    public readonly static Action<GoogleOptions, AuthSettings> ConfigureOptions = (options, authSettings) =>
    {
        options.ClientId = authSettings.OAuth!.Google!.ClientId;
        options.ClientSecret = authSettings.OAuth!.Google!.ClientSecret;

        options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.UserName), "name");
        options.ClaimActions.MapJsonKey(nameof(AuthenticatedSession.Email), "email");

        options.Events = new()
        {
            OnCreatingTicket = context =>
            {
                context.Identity?.AddClaim(new(
                    nameof(AuthenticatedSession.Status), OAuthUserStatus.Active.ToString()
                ));

                return Task.CompletedTask;
            }
        };
    };
}
