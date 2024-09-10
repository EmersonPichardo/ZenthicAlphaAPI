using Domain.Entities.Abstractions;

namespace Domain.Entities.Implementations;

public abstract class BaseCompoundEntity
    : BaseEntity, ICompoundEntity
{
    public int ClusterId { get; init; }
}