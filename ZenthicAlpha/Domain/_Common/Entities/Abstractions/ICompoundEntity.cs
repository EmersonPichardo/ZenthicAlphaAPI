namespace Domain._Common.Entities.Abstractions;

public interface ICompoundEntity : IEntity
{
    public int ClusterId { get; init; }
}
