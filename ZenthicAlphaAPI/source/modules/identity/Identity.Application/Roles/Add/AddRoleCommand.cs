using Application.Authorization;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Roles.Add;

[Authorize(Component.Roles, Permission.Add), Cache(Component.Roles)]
public record AddRoleCommand
    : ICommand
{
    public required string Name { get; init; }
    public required bool[][] SelectedPermissions { get; init; }
}
