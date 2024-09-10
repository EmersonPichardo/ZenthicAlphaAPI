using Domain.Modularity;
using Identity.Application.Users.Logout;
using MediatR;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Users;

public record LogoutCurrentUserEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public string Route { get; init; } = "/logout";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = null;
    public Delegate Handler { get; init; } = async (
        ISender mediator) =>
    {
        var command = new LogoutCurrentUserCommand();
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
