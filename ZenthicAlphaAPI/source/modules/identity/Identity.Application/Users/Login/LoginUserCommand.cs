using Application.Authorization;
using Application.Commands;

namespace Identity.Application.Users.Login;

[AllowAnonymous]
public record LoginUserCommand
    : ICommand<LoginUserCommandResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
