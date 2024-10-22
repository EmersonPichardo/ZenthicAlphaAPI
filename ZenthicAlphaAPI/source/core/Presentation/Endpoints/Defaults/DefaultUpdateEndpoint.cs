using Application.Commands;
using Domain.Modularity;
using MediatR;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultUpdateEndpoint<TCommand>(Component Component) : IEndpoint
    where TCommand : IUpdateCommand
{
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Put;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/{id:guid}"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [];
    public Delegate Handler { get; init; } = async (
        ISender mediator, Guid id, TCommand command, CancellationToken cancellationToken) =>
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
