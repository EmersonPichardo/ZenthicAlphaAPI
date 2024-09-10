using Domain.Modularity;
using Identity.Application.Users.ResetPassword;
using MediatR;
using Presentation.Endpoints;
using System.Net;

namespace Identity.Presentation.Users;

public record ResetUserPasswordEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Patch;
    public string Route { get; init; } = "/password/reset";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = null;
    public Delegate Handler { get; init; } = async (
        ISender mediator, ResetUserPasswordCommand command) =>
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
