using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Roles.Delete;

[Authorize(Component.Roles, Permission.Delete), Cache(Component.Roles)]
public record DeleteRoleCommand
    : BaseDeleteCommand;
