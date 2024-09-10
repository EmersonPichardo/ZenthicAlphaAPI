using Application.Authorization;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Roles.GetAll;

[Authorize(Component.Roles, Permission.Read), Cache(Component.Roles)]
public record GetAllRolesQuery
    : GetAllEntitiesQuery<GetAllRolesQueryResponse>;
