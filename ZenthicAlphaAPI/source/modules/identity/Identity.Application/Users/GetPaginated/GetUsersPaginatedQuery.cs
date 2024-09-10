using Application.Authorization;
using Application.Caching;
using Application.Pagination;
using Domain.Modularity;

namespace Identity.Application.Users.GetPaginated;

[Authorize(Component.Users, Permission.Read), Cache(Component.Users)]
public record GetUsersPaginatedQuery
    : GetEntitiesPaginatedQuery<GetUsersPaginatedQueryResponse>;