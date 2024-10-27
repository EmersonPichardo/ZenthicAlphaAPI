using Domain.Modularity;
using Identity.Application.Auth.OAuthCallback;
using Identity.Application.Common.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Identity.Presentation.Auth;

public record OAuthCallbackEndpoint : IEndpoint
{
    public Component Component { get; init; } = 0;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = [$"{OAuthConstants.CallbackPath}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender sender, HttpContext httpContext, string authenticationScheme, string redirectUrl, CancellationToken cancellationToken) =>
    {
        var command = new OAuthCallbackCommand
        {
            AuthenticationScheme = authenticationScheme
        };

        var result = await sender.Send(command, cancellationToken);

        var response = result.Match<OneOf<OAuthCallbackCommandResponse, ProblemDetails>>(
            successResponse => successResponse,
            failure => ProblemFactory.CreateFromFailure(failure)
        );

        var jsonResponse = JsonSerializer.Serialize(response.Value);
        var jsonResponseBytes = Encoding.Default.GetBytes(jsonResponse);
        var base64Response = Convert.ToBase64String(jsonResponseBytes);

        foreach (var cookie in httpContext.Request.Cookies)
            httpContext.Response.Cookies.Delete(cookie.Key);

        return Results.Redirect(
            $"{redirectUrl}?encodedResponse={base64Response}", true, true
        );
    };
}

