using Domain.Modularity;
using Identity.Application.Roles.GetAll;
using Identity.Application.Roles.GetPaginated;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record GetRolesEndpoint() : DefaultGetListEndpoint<
    GetAllRolesQuery, GetAllRolesQueryResponse,
    GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse
>(Component.Roles);
