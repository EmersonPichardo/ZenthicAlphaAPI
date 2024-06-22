using Application._Common.Caching;
using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Roles.GetAll;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetAllRolesQuery
    : GetAllEntitiesQuery<GetAllRolesQueryResponse>;
