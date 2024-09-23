namespace Domain.Entities.Abstractions;

public interface ICompoundEntity : IEntity
{
    public int ClusterId { get; init; }
}
