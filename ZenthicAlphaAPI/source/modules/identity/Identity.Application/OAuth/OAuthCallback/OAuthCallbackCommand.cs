using Application.Auth;
using Application.Commands;

namespace Identity.Application.OAuth.OAuthCallback;

[AllowAnonymous]
public record OAuthCallbackCommand
    : ICommand<string>
{
    public required string AuthenticationScheme { get; init; }
}
