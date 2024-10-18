using Application.Commands;
using Domain.Modularity;
using MediatR;
using OneOf.Types;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultAddEndpoint<TCommand, TResponse>(Component Component) : IEndpoint
    where TCommand : ICommand<TResponse>
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.Created;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender mediator, TCommand command, CancellationToken cancellationToken) =>
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Created,
            ResultFactory.ProblemDetails
        );
    };
}

public abstract record DefaultAddEndpoint<TCommand>(Component Component)
    : DefaultAddEndpoint<TCommand, Success>(Component)
    where TCommand : ICommand<Success>;
