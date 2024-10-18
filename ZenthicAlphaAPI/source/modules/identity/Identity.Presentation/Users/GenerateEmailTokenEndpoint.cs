using Domain.Modularity;
using Identity.Application.Users.GenerateEmailToken;
using MediatR;
using OneOf.Types;
using Presentation.Endpoints;
using Presentation.Result;
using System.Net;

namespace Identity.Presentation.Users;

public record GenerateEmailTokenEndpoint : IEndpoint
{
    public Component Component { get; init; } = Component.Users;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/me/email/send-confirmation-token"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender mediator, CancellationToken cancellationToken) =>
    {
        var command = new GenerateEmailTokenCommand() { UserId = null };
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            token => ResultFactory.Ok(new Success()),
            ResultFactory.ProblemDetails
        );
    };
}
