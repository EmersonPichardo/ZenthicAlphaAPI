namespace Application.Pagination;

public interface IGetEntitiesPaginatedQuery
{
    string? Search { get; init; }
    int? CurrentPage { get; init; }
    int? PageSize { get; init; }
}