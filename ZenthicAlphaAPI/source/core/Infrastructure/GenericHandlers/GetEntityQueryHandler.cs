using Application.Failures;
using Application.Persistence.Databases;
using Application.Queries;
using Domain.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OneOf;

namespace Infrastructure.GenericHandlers;

internal abstract class GetEntityQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? getIncludesExpression = null
)
    where TQuery : GetEntityQuery<TResponse>
    where TResponse : class
    where TEntity : class, IEntity
{
    protected abstract TResponse MapToResponse(TEntity entity);

    public async Task<OneOf<TResponse, Failure>> Handle(
        TQuery query,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution();

        if (getIncludesExpression is not null)
            entities = getIncludesExpression(entities);

        var entity = await entities
            .FirstOrDefaultAsync(
                entity => entity.Id == query.Id,
                cancellationToken
            );

        if (entity is null)
            return FailureFactory.NotFound(
                $"{typeof(TEntity).Name} not found",
                $"No {typeof(TEntity).Name.ToLower()} was found with an Id of {query.Id}"
            );

        return MapToResponse(entity);
    }
}