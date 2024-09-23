using Application.Commands;
using Domain.Modularity;
using MediatR;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultDeleteEndpoint<TCommand>(Component Component) : IEndpoint
    where TCommand : IDeleteCommand, new()
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Delete;
    public string Route { get; init; } = "/";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = null;
    public Delegate Handler { get; init; } = async (
        ISender mediator, Guid id) =>
    {
        var command = new TCommand() { Id = id };
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
