using Domain.Modularity;
using Identity.Application.Users.ChangePassword;
using MediatR;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Users;

public record ChangeUserPasswordEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Patch;
    public string Route { get; init; } = "/password";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = null;
    public Delegate Handler { get; init; } = async (
        ISender mediator, ChangeUserPasswordCommand command) =>
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
