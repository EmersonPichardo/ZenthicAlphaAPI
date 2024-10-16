using Application.Auth;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Roles.Delete;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Delete)]
public record DeleteRoleCommand
    : BaseDeleteCommand;
