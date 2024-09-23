namespace Domain.Entities.Abstractions;

public interface IEntity
{
    public Guid Id { get; init; }
}