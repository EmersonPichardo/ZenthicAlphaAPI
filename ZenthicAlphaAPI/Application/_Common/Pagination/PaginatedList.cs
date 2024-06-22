using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application._Common.Pagination;

public class PaginatedList<T>
    where T : class
{
    public required IReadOnlyCollection<T> Items { get; init; } = new List<T>();
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        Expression<Func<T, bool>> filterPredicate,
        int? currentPage,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var currentPageValue = currentPage.GetValueOrDefault(1);
        var pageSizeValue = pageSize.GetValueOrDefault(10);

        if (currentPageValue < 1)
            return new PaginatedList<T>()
            {
                Items = new List<T>(),
                TotalCount = 0,
                PageSize = pageSizeValue,
                CurrentPage = currentPageValue
            };

        var filteredItems = source
            .Where(filterPredicate);

        var count = await filteredItems
            .CountAsync(cancellationToken);

        var paginatedItems = await filteredItems
            .Skip((currentPageValue - 1) * pageSizeValue)
            .Take(pageSizeValue)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>()
        {
            Items = paginatedItems,
            TotalCount = count,
            PageSize = pageSizeValue,
            CurrentPage = currentPageValue
        };
    }
}
