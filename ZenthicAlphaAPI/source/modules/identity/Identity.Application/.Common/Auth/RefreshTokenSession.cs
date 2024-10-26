using Application.Auth;

namespace Identity.Application.Common.Auth;

public record RefreshTokenSession : IUserSession
{
    public required Guid UserId { get; init; }
}
