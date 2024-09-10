using Application.Authorization;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Users.GetAll;

[Authorize(Component.Users, Permission.Read), Cache(Component.Users)]
public record GetAllUsersQuery
    : GetAllEntitiesQuery<GetAllUsersQueryResponse>;
