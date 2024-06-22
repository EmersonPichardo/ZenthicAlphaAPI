using Application._Common.Security.Authorization;

namespace Application.Users.Login;

[AllowAnonymous]
public record LoginUserCommand
    : ICommand<LoginUserCommandResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
