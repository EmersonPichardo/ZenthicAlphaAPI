using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.UpdateOAuthUser;

[AuthorizeOAuth]
public record UpdateOAuthUserCommand
    : ICommand
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
}

