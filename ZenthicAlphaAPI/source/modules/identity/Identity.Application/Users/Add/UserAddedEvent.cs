using Application.Events;

namespace Identity.Application.Users.Add;

public record UserAddedEvent
    : IEvent
{
    public required UserDto User { get; init; }
}
