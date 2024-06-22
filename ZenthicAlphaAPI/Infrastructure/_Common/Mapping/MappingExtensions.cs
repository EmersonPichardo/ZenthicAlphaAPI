using Application._Common.Pagination;
using System.Linq.Expressions;

namespace Infrastructure._Common.Mapping;

internal static class MappingExtensions
{
    internal static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        Expression<Func<TDestination, bool>> filterPredicate,
        int? currentPage,
        int? pageSize,
        CancellationToken cancellationToken = default)
        where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(
            queryable,
            filterPredicate,
            currentPage,
            pageSize,
            cancellationToken
        );
    }
}

