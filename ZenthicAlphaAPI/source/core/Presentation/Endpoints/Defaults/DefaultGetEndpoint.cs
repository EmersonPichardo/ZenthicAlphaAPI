using Application.Queries;
using Domain.Modularity;
using MediatR;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetEntityQuery<TResponse>, new()
    where TResponse : class
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public string Route { get; init; } = "/{id:guid}";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = typeof(TResponse);
    public Delegate Handler { get; init; } = async (
        ISender mediator, Guid id) =>
    {
        var query = new TQuery() { Id = id };
        var result = await mediator.Send(query);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
