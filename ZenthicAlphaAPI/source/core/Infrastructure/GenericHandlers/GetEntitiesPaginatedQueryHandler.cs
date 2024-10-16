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
    IApplicationDbContext dbContext
)
    where TQuery : GetEntitiesPaginatedQuery<TResponse>
    where TResponse : class
    where TEntity : class, ICompoundEntity
{
    protected abstract Expression<Func<TResponse, bool>> GetFilterExpression(string? filter);
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
                GetFilterExpression(query.Filter),
                query.CurrentPage,
                query.PageSize,
                cancellationToken
            );
    }
}