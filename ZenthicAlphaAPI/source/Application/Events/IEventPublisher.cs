namespace Application.Events;

public interface IEventPublisher
{
    void EnqueueEvent(IEvent @event);
    void EnqueueEvents(IEnumerable<IEvent> events);
    IEnumerable<IEvent> GetPendingEvents();
    bool HasPendingEvents();
    bool HasNoPendingEvents() => !HasPendingEvents();
}
