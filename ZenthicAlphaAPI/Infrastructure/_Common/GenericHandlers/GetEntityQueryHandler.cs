using Application._Common.Exceptions;
using Application._Common.Persistence.Databases;
using Application._Common.Queries;
using AutoMapper;
using Domain._Common.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
    public async Task<TResponse> Handle(
        TQuery query,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution()
            .AsSplitQuery();

        if (getIncludesExpression is not null)
            entities = getIncludesExpression(entities);

        var entity = await entities
            .FirstOrDefaultAsync(
                entity => entity.Id.Equals(query.Id),
                cancellationToken
            )
        ?? throw new NotFoundException(typeof(TEntity).Name, query.Id);

        return mapper.Map<TResponse>(entity);
    }
}