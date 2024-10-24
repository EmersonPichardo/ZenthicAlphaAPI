using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.AddOAuthUser;

[AuthorizeOAuth]
public record AddOAuthUserCommand
    : ICommand<AddOAuthUserCommandResponse>
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}
