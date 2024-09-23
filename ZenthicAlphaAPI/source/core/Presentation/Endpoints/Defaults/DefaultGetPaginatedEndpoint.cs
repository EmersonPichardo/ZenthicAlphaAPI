using Application.Pagination;
using Domain.Modularity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetPaginatedEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetEntitiesPaginatedQuery<TResponse>, new()
    where TResponse : class
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public string Route { get; init; } = "/paginated";
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public Type? SuccessType { get; init; } = typeof(PaginatedList<TResponse>);
    public Delegate Handler { get; init; } = async (
        ISender mediator, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? search) =>
    {
        var query = new TQuery()
        {
            Search = search,
            CurrentPage = page,
            PageSize = size
        };

        var result = await mediator.Send(query);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    };
}