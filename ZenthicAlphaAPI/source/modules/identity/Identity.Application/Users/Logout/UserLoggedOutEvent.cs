using Application.Events;
using Identity.Application._Common.Authentication;

namespace Identity.Application.Users.Logout;

public record UserLoggedOutEvent
    : IEvent
{
    public required ICurrentUser User { get; init; }
}
