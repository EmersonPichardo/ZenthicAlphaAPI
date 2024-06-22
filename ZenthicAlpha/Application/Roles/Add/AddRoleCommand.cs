using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Roles.Add;

[Authorize(Component.Roles, Permission.Add), Cache(Component.Roles)]
public record AddRoleCommand
    : ICommand
{
    public required string Name { get; init; }
    public required bool[][] SelectedPermissions { get; init; }
}
