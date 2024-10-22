using Application.Pagination;
using Domain.Modularity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetPaginatedEndpoint<TQuery, TResponse>(Component Component) : IEndpoint
    where TQuery : GetEntitiesPaginatedQuery<TResponse>, new()
{
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(PaginatedList<TResponse>)];
    public Delegate Handler { get; init; } = async (
        ISender mediator, int? page, int? pageSize, string? filter, CancellationToken cancellationToken) =>
    {
        return await GetPaginatedResultAsync(mediator, page, pageSize, filter, cancellationToken);
    };

    public static async Task<IResult> GetPaginatedResultAsync(ISender mediator, int? page, int? pageSize, string? filter, CancellationToken cancellationToken)
    {
        var query = new TQuery
        {
            Filter = filter,
            CurrentPage = page,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
}