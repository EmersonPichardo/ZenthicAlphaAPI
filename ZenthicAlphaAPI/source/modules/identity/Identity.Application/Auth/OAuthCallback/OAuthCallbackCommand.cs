using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.OAuthCallback;

[AuthorizeOAuth]
public record OAuthCallbackCommand
    : ICommand<string>
{
    public required string AuthenticationScheme { get; init; }
}
