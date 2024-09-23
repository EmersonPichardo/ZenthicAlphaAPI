using Domain.Entities.Abstractions;
using MediatR;

namespace Application.Events;

public interface IEvent
    : INotification;

public interface IEvent<TEntity>
    : IEvent
    where TEntity : IEntity
{
    public TEntity Entity { get; init; }
}