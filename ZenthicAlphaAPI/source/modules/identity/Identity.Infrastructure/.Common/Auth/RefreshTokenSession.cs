using Application.Auth;

namespace Identity.Infrastructure.Common.Auth;

internal record RefreshTokenSession : IUserSession
{
    public required Guid UserId { get; init; }
}
