using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Roles.Get;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetRoleQuery
    : GetEntityQuery<GetRoleQueryResponse>;
