using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.OAuthCallback;

[AuthorizeOAuth]
public record OAuthCallbackCommand
    : ICommand<OAuthCallbackCommandResponse>
{
    public required string AuthenticationScheme { get; init; }
}
