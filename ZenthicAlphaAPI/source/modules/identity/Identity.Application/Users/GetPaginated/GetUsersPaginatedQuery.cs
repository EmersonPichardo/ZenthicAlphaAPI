using Application.Auth;
using Application.Caching;
using Application.Pagination;
using Domain.Modularity;

namespace Identity.Application.Users.GetPaginated;

[Cache(Component.Users)]
[Authorize(Component.Users, Permission.Read)]
public record GetUsersPaginatedQuery
    : GetEntitiesPaginatedQuery<GetUsersPaginatedQueryResponse>;