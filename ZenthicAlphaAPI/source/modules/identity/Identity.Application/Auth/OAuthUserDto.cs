using Identity.Domain.User;

namespace Identity.Application.Auth;

public record OAuthUserDto
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }

    public static OAuthUserDto FromOAuthUser(OAuthUser oauthUser) => new()
    {
        Id = oauthUser.Id,
        UserName = oauthUser.UserName,
        Email = oauthUser.Email,
        Status = oauthUser.Status.ToString()
    };
}
