using Application.Failures;
using Application.Pagination;
using Application.Queries;
using Domain.Modularity;
using MediatR;
using Presentation.Result;
using System.Net;

namespace Presentation.Endpoints.Defaults;

public abstract record DefaultGetListEndpoint<
    TGetAllQuery, TGetAllResponse,
    TGetPaginatedQuery, TGetPaginatedResponse>(Component Component)
: IEndpoint
    where TGetAllQuery : GetAllEntitiesQuery<TGetAllResponse>, new()
    where TGetPaginatedQuery : GetEntitiesPaginatedQuery<TGetPaginatedResponse>, new()
{
    public Component Component { get; init; } = Component;
    public HttpVerbose Verbose { get; init; } = HttpVerbose.Get;
    public IReadOnlyCollection<string> Routes { get; init; } = ["/"];
    public HttpStatusCode SuccessStatusCode { get; init; } = HttpStatusCode.OK;
    public IReadOnlyCollection<Type> SuccessTypes { get; init; } = [typeof(TGetAllResponse), typeof(TGetPaginatedResponse)];
    public Delegate Handler { get; init; } = async (
        ISender mediator, int? page, int? pageSize, string? filter, CancellationToken cancellationToken) =>
    {
        QueryType GetQueryType()
        {
            if (page is not null || pageSize is not null || !string.IsNullOrEmpty(filter))
                return QueryType.GetPaginated;

            return QueryType.GetAll;
        }

        return GetQueryType() switch
        {
            QueryType.GetAll => await DefaultGetAllEndpoint<TGetAllQuery, TGetAllResponse>.GetAllQueryResultAsync(mediator, cancellationToken),
            QueryType.GetPaginated => await DefaultGetPaginatedEndpoint<TGetPaginatedQuery, TGetPaginatedResponse>.GetPaginatedResultAsync(mediator, page, pageSize, filter, cancellationToken),
            _ => ResultFactory.ProblemDetails(FailureFactory.InvalidRequest("Incorrect request", "Unknown request structure"))
        };
    };

    private enum QueryType
    {
        GetAll,
        GetPaginated
    }
}
