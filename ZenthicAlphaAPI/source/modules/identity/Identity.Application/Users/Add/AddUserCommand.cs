using Application.Auth;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.Add;

[Cache(Component.Users)]
[AllowAnonymous]
public record AddUserCommand
    : ICommand<AddUserCommandResponse>
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string RepeatedPassword { get; init; }
    public required IReadOnlyList<Guid> RoleIds { get; init; }
}
