using Application.Auth;
using Application.Caching;
using Application.Pagination;
using Domain.Modularity;

namespace Identity.Application.Roles.GetPaginated;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Read)]
public record GetRolesPaginatedQuery
    : GetEntitiesPaginatedQuery<GetRolesPaginatedQueryResponse>;