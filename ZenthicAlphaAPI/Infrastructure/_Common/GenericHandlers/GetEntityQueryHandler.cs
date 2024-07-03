using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application._Common.Queries;
using AutoMapper;
using Domain._Common.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OneOf;

namespace Infrastructure._Common.GenericHandlers;

internal abstract class GetEntityQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    IMapper mapper,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? getIncludesExpression = null
)
    where TQuery : GetEntityQuery<TResponse>
    where TResponse : class
    where TEntity : class, IEntity
{
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
                entity => entity.Id.Equals(query.Id),
                cancellationToken
            );

        if (entity is null)
            return FailureFactory.NotFound(
                $"{typeof(TEntity).Name} not found",
                $"No {typeof(TEntity).Name.ToLower()} was found with an Id of {query.Id}"
            );

        return mapper.Map<TResponse>(entity);
    }
}