using Application.Authorization;
using Application.Caching;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Roles.Delete;

[Authorize(Component.Roles, Permission.Delete), Cache(Component.Roles)]
public record DeleteRoleCommand
    : BaseDeleteCommand;
