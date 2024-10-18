using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.Update;

[Cache(Component.Users)]
public record UpdateUserCommand
    : ICommand
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
}
