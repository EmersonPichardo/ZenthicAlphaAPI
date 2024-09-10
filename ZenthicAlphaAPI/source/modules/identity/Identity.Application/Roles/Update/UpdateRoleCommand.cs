using Application.Authorization;
using Application.Caching;
using Domain.Modularity;
using Identity.Application.Roles.Add;

namespace Identity.Application.Roles.Update;

[Authorize(Component.Roles, Permission.Update), Cache(Component.Roles)]
public record UpdateRoleCommand
    : AddRoleCommand
{
    public required Guid Id { get; init; }
}
