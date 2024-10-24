using Domain.Modularity;
using Identity.Application.Auth.OAuthCallback;
using Identity.Application.Common.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        ISender sender, string authenticationScheme, string redirectUrl, CancellationToken cancellationToken) =>
    {
        var command = new OAuthCallbackCommand
        {
            AuthenticationScheme = authenticationScheme
        };

        var result = await sender.Send(command, cancellationToken);

        var response = result.Match<object>(
            successResponse => successResponse,
            ProblemFactory.CreateFromFailure
        );

        var jsonResponse = JsonSerializer.Serialize(response);
        var jsonResponseBytes = Encoding.Default.GetBytes(jsonResponse);
        var base64Response = Convert.ToBase64String(jsonResponseBytes);

        return Results.Redirect(
            $"{redirectUrl}?base64-response={base64Response}", true, true
        );
    };
}

