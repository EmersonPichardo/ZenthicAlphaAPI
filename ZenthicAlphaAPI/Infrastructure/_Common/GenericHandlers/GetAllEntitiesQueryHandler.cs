using Application._Common.Persistence.Databases;
using Application._Common.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain._Common.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure._Common.GenericHandlers;

internal abstract class GetAllEntitiesQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    IMapper mapper
)
    where TQuery : GetAllEntitiesQuery<TResponse>
    where TResponse : class
    where TEntity : class, IEntity
{
    public async Task<IList<TResponse>> Handle(
        TQuery _,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution()
            .AsSplitQuery();

        return await entities
            .ProjectTo<TResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}