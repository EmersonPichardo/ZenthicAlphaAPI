using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Users.Add;

[Authorize(Component.Users, Permission.Add), Cache(Component.Users)]
public record AddUserCommand
    : ICommand
{
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyList<Guid> RoleIds { get; init; }
}
