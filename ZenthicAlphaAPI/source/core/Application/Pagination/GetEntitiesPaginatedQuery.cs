using Application.Helpers;
using Application.Queries;

namespace Application.Pagination;

public record GetEntitiesPaginatedQuery<TResponse>
    : IGetEntitiesPaginatedQuery
    , IQuery<PaginatedList<TResponse>>
{
    public string? Filter { get => filter; init => filter = value?.ToNormalize(); }
    private string? filter;
    public int? CurrentPage { get; init; }
    public int? PageSize { get; init; }
}