using Application._Common.Caching;
using Application._Common.Pagination;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Users.GetPaginated;

[Authorize(Component.Users, Permission.Read), Cache(Component.Users)]
public record GetUsersPaginatedQuery
    : GetEntitiesPaginatedQuery<GetUsersPaginatedQueryResponse>;