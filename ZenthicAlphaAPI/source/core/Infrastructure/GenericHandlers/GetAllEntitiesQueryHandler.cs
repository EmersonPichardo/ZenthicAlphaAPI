using Application.Failures;
using Application.Persistence.Databases;
using Application.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Infrastructure.GenericHandlers;

public abstract class GetAllEntitiesQueryHandler<TQuery, TResponse, TEntity>(
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