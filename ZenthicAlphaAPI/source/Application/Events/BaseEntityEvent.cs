using Domain.Entities.Abstractions;

namespace Application.Events;

public record BaseEntityEvent<TEntity>
    : IEvent<TEntity>
    where TEntity : IEntity
{
    public required TEntity Entity { get; init; }
}
