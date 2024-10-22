using Domain.Modularity;
using Identity.Application.Common.Auth;
using Identity.Application.OAuth.OAuthCallback;
using MediatR;
using Microsoft.AspNetCore.Http;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Auth;

public record OAuthCallbackEndpoint : IEndpoint
{
    public Component Component { get; init; } = 0;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = [$"{OAuthConstants.CallbackPath}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender mediator, string authenticationScheme, CancellationToken cancellationToken) =>
    {
        var command = new OAuthCallbackCommand
        {
            AuthenticationScheme = authenticationScheme
        };

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            redirectUrl => Results.Redirect(redirectUrl, true, true),
            ResultFactory.ProblemDetails
        );
    };
}

