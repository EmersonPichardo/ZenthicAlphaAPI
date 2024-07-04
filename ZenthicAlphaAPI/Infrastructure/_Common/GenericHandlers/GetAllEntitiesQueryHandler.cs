using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application._Common.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain._Common.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Infrastructure._Common.GenericHandlers;

internal abstract class GetAllEntitiesQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext,
    IMapper mapper
)
    where TQuery : GetAllEntitiesQuery<TResponse>
    where TResponse : class
    where TEntity : class, IEntity
{
    public async Task<OneOf<IList<TResponse>, Failure>> Handle(
        TQuery _,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution();

        return await entities
            .ProjectTo<TResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}