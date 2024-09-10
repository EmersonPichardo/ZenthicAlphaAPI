using Domain.Modularity;
using Identity.Application.Users.Login;
using MediatR;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Users;

public record LoginUserEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public string Route { get; init; } = "/login";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = typeof(LoginUserCommandResponse);
    public Delegate Handler { get; init; } = async (
        ISender mediator, LoginUserCommand command) =>
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
