using Application.Helpers;
using Application.Queries;

namespace Application.Pagination;

public record GetEntitiesPaginatedQuery<TResponse>
    : IGetEntitiesPaginatedQuery
    , IQuery<PaginatedList<TResponse>>
where TResponse : class
{
    public string? Search { get => search; init => search = value?.ToNormalize(); }
    private string? search;
    public int? CurrentPage { get; init; }
    public int? PageSize { get; init; }
}