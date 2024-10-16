namespace Application.Pagination;

public interface IGetEntitiesPaginatedQuery
{
    string? Filter { get; init; }
    int? CurrentPage { get; init; }
    int? PageSize { get; init; }
}