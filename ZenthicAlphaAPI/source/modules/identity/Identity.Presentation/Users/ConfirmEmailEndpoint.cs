using Domain.Modularity;
using Identity.Application.Users.ConfirmEmail;
using MediatR;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Users;

public record ConfirmEmailEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/me/email/confirm"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender mediator, string token, CancellationToken cancellationToken
    ) =>
    {
        var command = new ConfirmEmailCommand { Token = token };
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
