using Application._Common.Security.Authorization;

namespace Application.Users.ClearSession;

[AllowAnonymous]
public record ClearUserSessionCommand
    : ICommand
{
    public required Guid UserId { get; init; }
}
