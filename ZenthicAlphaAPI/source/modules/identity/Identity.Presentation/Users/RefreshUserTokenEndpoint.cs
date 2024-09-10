using Domain.Modularity;
using Identity.Application.Users.RefreshToken;
using MediatR;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Users;

public record RefreshUserTokenEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Patch;
    public string Route { get; init; } = "/token";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = typeof(RefreshUserTokenCommandResponse);
    public Delegate Handler { get; init; } = async (
        ISender mediator) =>
    {
        var command = new RefreshUserTokenCommand();
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
