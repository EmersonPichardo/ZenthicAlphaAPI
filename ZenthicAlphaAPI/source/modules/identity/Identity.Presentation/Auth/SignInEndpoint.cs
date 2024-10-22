using Domain.Modularity;
using Identity.Application.Users.Login;
using MediatR;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Auth;

public record SignInEndpoint : IEndpoint
{
    public Component Component { get; init; } = 0;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/sign-in"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(LoginUserCommandResponse)];
    public Delegate Handler { get; init; } = async (
        ISender mediator, LoginUserCommand command, CancellationToken cancellationToken) =>
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
