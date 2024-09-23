using Application.Pagination;
using System.Linq.Expressions;

namespace Infrastructure.Mapping;

internal static class MappingExtensions
{
    internal static IQueryable<TDestination> ProjectTo<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TDestination>> projection
    )
        where TDestination : class
    {
        return queryable.Select(projection);
    }

    internal static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        Expression<Func<TDestination, bool>> filterPredicate,
        int? currentPage,
        int? pageSize,
        CancellationToken cancellationToken = default
    )
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

