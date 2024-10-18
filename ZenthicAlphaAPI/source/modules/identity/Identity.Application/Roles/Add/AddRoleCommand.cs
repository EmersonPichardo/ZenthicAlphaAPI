using Application.Auth;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Roles.Add;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Add)]
public record AddRoleCommand
    : ICommand<AddRoleCommandResponse>
{
    public required string Name { get; init; }
    public required IReadOnlyDictionary<string, string[]> SelectedPermissions { get; init; }
}
