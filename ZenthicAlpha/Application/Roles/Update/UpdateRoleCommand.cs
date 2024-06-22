using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Application.Roles.Add;
using Domain._Common.Modularity;

namespace Application.Roles.Update;

[Authorize(Component.Roles, Permission.Update), Cache(Component.Roles)]
public record UpdateRoleCommand
    : AddRoleCommand
{
    public required Guid Id { get; init; }
}
