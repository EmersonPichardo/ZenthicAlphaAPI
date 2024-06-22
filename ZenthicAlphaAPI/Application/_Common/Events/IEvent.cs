using Domain._Common.Entities.Abstractions;
using MediatR;

namespace Application._Common.Events;

public interface IEvent
    : INotification;

public interface IEvent<TEntity>
    : IEvent
    where TEntity : IEntity
{
    public TEntity Entity { get; init; }
}