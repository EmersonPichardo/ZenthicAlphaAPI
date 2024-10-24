using Domain.Modularity;
using Identity.Application.Users.RefreshToken;
using MediatR;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Auth;

public record RefreshTokenEndpoint : IEndpoint
{
    public Component Component { get; init; } = 0;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/refresh-token"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(RefreshUserTokenCommandResponse)];
    public Delegate Handler { get; init; } = async (
        ISender sender, CancellationToken cancellationToken) =>
    {
        var command = new RefreshUserTokenCommand();
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
