using Domain.Identity;

namespace Application.Auth;

public record AuthenticatedSession : IUserSession
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required UserStatus Status { get; init; }
}
