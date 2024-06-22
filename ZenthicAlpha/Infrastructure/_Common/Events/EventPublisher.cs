using Application._Common.Events;
using System.Collections.Concurrent;

namespace Infrastructure._Common.Events;

internal class EventPublisher : IEventPublisher
{
    private readonly ConcurrentQueue<IEvent> events = [];

    public void EnqueueEvent(IEvent @event)
        => events.Enqueue(@event);

    public void EnqueueEvents(IEnumerable<IEvent> events)
    {
        foreach (var @event in events)
            this.events.Enqueue(@event);
    }

    public IEnumerable<IEvent> GetPendingEvents()
    {
        while (events.TryDequeue(out var @event))
            yield return @event;
    }

    public bool HasPendingEvents()
        => !events.IsEmpty;
}
