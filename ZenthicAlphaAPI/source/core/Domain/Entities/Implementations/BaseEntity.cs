using Domain.Entities.Abstractions;

namespace Domain.Entities.Implementations;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
