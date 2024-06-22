using Domain._Common.Entities.Abstractions;

namespace Application._Common.Events;

public record BaseEntityEvent<TEntity>
    : IEvent<TEntity>
    where TEntity : IEntity
{
    public required TEntity Entity { get; init; }
}
