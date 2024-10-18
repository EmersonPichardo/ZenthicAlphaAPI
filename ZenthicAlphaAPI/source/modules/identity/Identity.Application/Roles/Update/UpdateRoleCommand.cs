using Application.Auth;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;
using Identity.Application.Roles.Add;

namespace Identity.Application.Roles.Update;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Update)]
public record UpdateRoleCommand
    : AddRoleCommand, IUpdateCommand
{
    public Guid Id { get; set; }
}
