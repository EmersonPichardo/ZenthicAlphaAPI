using Application.Failures;
using Application.Pagination;
using Application.Persistence.Databases;
using Domain.Entities.Abstractions;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq.Expressions;

namespace Infrastructure.GenericHandlers;

public abstract class GetEntitiesPaginatedQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    Func<string?, Expression<Func<TResponse, bool>>> getFilterExpression
)
    where TQuery : GetEntitiesPaginatedQuery<TResponse>
    where TResponse : class
    where TEntity : class, ICompoundEntity
{
    protected abstract Expression<Func<TEntity, TResponse>> MapToResponse();

    public async Task<OneOf<PaginatedList<TResponse>, Failure>> Handle(
        TQuery query,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution();

        return await entities
            .OrderBy(entity => entity.ClusterId)
            .ProjectTo(MapToResponse())
            .PaginatedListAsync(
                getFilterExpression(query.Search),
                query.CurrentPage,
                query.PageSize,
                cancellationToken
            );
    }
}