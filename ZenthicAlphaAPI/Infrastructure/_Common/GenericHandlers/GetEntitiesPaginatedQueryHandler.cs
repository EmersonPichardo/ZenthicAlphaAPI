using Application._Common.Pagination;
using Application._Common.Persistence.Databases;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain._Common.Entities.Abstractions;
using Infrastructure._Common.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure._Common.GenericHandlers;

internal abstract class GetEntitiesPaginatedQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    IMapper mapper,
    Func<string?, Expression<Func<TResponse, bool>>> getFilterExpression
)
    where TQuery : GetEntitiesPaginatedQuery<TResponse>
    where TResponse : class
    where TEntity : class, ICompoundEntity
{
    public async Task<PaginatedList<TResponse>> Handle(
        TQuery query,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution()
            .AsSplitQuery();

        return await entities
            .OrderBy(entity => entity.ClusterId)
            .ProjectTo<TResponse>(mapper.ConfigurationProvider)
            .PaginatedListAsync(
                getFilterExpression(query.Search),
                query.CurrentPage,
                query.PageSize,
                cancellationToken
            );
    }
}