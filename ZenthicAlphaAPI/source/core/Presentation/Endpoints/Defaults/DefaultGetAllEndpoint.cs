using Application.Queries;
using Domain.Modularity;
using MediatR;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetAllEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetAllEntitiesQuery<TResponse>, new()
    where TResponse : class
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public string Route { get; init; } = "/";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = typeof(IEnumerable<TResponse>);
    public Delegate Handler { get; init; } = async (
        ISender mediator) =>
    {
        var query = new TQuery();
        var result = await mediator.Send(query);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}
