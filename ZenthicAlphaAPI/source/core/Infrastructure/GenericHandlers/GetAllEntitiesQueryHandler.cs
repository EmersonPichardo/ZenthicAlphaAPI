using Application.Failures;
using Application.Persistence.Databases;
using Application.Queries;
using Domain.Entities.Abstractions;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using OneOf;
using System.Linq.Expressions;

namespace Infrastructure.GenericHandlers;

public abstract class GetAllEntitiesQueryHandler<TQuery, TResponse, TEntity>(
    IApplicationDbContext dbContext
)
    where TQuery : GetAllEntitiesQuery<TResponse>
    where TResponse : class
    where TEntity : class, IEntity
{
    protected abstract Expression<Func<TEntity, TResponse>> MapToResponse();

    public async Task<OneOf<IList<TResponse>, Failure>> Handle(
        TQuery _,
        CancellationToken cancellationToken)
    {
        var entities = dbContext
            .Set<TEntity>()
            .AsNoTrackingWithIdentityResolution();

        return await entities
            .ProjectTo(MapToResponse())
            .ToListAsync(cancellationToken);
    }
}