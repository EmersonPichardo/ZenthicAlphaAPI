using Application.Authorization;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Roles.Get;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetRoleQuery
    : GetEntityQuery<GetRoleQueryResponse>;
