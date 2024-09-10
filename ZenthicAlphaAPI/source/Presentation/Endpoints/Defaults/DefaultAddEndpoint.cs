using Application.Commands;
using Domain.Modularity;
using MediatR;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultAddEndpoint<TCommand>(Component Component) : IEndpoint
    where TCommand : ICommand
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Post;
    public string Route { get; init; } = "/";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.Created;
    public Type? SuccessType { get; init; } = null;
    public Delegate Handler { get; init; } = async (
        ISender mediator, TCommand command) =>
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Created,
            ResultFactory.ProblemDetails
        );
    };
}
