using Application.Auth;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Roles.GetAll;

[Cache(Component.Roles)]
[Authorize(Component.Roles, Permission.Read)]
public record GetAllRolesQuery
    : GetAllEntitiesQuery<GetAllRolesQueryResponse>;
