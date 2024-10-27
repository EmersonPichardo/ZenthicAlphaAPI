using Domain.Modularity;
using Identity.Application.Common.Auth;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Auth;

public record GoogleSignInEndpoint : IEndpoint
{
    public Component Component { get; init; } = 0;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = [$"/sign-in/{GoogleDefaults.AuthenticationScheme.ToLower()}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = (
        string redirectUrl) =>
    {
        return Results.Challenge(
            new()
            {
                RedirectUri =
                    $"/api{OAuthConstants.CallbackPath}" +
                    $"?authenticationScheme={GoogleDefaults.AuthenticationScheme}" +
                    $"&redirectUrl={redirectUrl}"
            },
                [GoogleDefaults.AuthenticationScheme]
        );
    };
}

