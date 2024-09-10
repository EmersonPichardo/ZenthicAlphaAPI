using Domain.Modularity;
using Identity.Application.Roles.GetAll;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record GetAllRolesEndpoint : DefaultGetAllEndpoint<GetAllRolesQuery, GetAllRolesQueryResponse>
{
    public GetAllRolesEndpoint() : base(Component.Roles) { }
}
