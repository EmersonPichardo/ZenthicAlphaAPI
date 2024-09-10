using Application.Authorization;
using Application.Caching;
using Application.Pagination;
using Domain.Modularity;

namespace Identity.Application.Roles.GetPaginated;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetRolesPaginatedQuery
    : GetEntitiesPaginatedQuery<GetRolesPaginatedQueryResponse>;