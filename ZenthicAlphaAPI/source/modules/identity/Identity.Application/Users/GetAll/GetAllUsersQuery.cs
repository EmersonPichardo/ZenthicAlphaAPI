using Application.Auth;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Users.GetAll;

[Cache(Component.Users)]
[Authorize(Component.Users, Permission.Read)]
public record GetAllUsersQuery
    : GetAllEntitiesQuery<GetAllUsersQueryResponse>;
