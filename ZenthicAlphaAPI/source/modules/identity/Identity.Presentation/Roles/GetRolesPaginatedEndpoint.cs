using Domain.Modularity;
using Identity.Application.Roles.GetPaginated;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record GetRolesPaginatedEndpoint : DefaultGetPaginatedEndpoint<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse>
{
    public GetRolesPaginatedEndpoint() : base(Component.Roles) { }
}
