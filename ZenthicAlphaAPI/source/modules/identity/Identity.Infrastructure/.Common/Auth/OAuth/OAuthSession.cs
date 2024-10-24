using Application.Auth;
using Identity.Domain.User;

namespace Identity.Infrastructure.Common.Auth;

public record OAuthSession : IUserSession
{
    public required string AuthenticationType { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required OAuthUserStatus Status { get; init; }
    public required string RedirectUrl { get; init; }
}
