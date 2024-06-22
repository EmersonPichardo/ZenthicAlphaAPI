using Application._Common.Security.Authentication;

namespace Application.Users.Logout;

public record UserLoggedOutEvent
    : IEvent
{
    public required ICurrentUser User { get; init; }
}
