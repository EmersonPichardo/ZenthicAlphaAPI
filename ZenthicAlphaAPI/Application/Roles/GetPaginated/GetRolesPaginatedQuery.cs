using Application._Common.Caching;
using Application._Common.Pagination;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Roles.GetPaginated;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetRolesPaginatedQuery
    : GetEntitiesPaginatedQuery<GetRolesPaginatedQueryResponse>;