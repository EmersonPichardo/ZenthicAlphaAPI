using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Users.GetAll;

[Authorize(Component.Users, Permission.Read), Cache(Component.Users)]
public record GetAllUsersQuery
    : GetAllEntitiesQuery<GetAllUsersQueryResponse>;
