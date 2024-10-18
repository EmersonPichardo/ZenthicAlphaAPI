using Application.Events;

namespace Identity.Application.Users.GenerateEmailToken;

public record EmailTokenGeneratedEvent
    : IEvent
{
    public required UserDto User { get; init; }
    public required string Token { get; init; }
}
