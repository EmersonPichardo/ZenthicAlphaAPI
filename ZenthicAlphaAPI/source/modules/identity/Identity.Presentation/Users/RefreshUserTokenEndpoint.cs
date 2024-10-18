using Domain.Modularity;
using Identity.Application.Users.RefreshToken;
using MediatR;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Users;

public record RefreshUserTokenEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/refresh-token"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(RefreshUserTokenCommandResponse)];
    public Delegate Handler { get; init; } = async (
        ISender mediator, CancellationToken cancellationToken) =>
    {
        var command = new RefreshUserTokenCommand();
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
