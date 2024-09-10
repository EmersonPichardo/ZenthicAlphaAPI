using Application.Authorization;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.Add;

[Authorize(Component.Users, Permission.Add), Cache(Component.Users)]
public record AddUserCommand
    : ICommand
{
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyList<Guid> RoleIds { get; init; }
}
