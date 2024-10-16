using Application.Auth;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Roles.Get;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Read)]
public record GetRoleQuery
    : GetEntityQuery<GetRoleQueryResponse>;
