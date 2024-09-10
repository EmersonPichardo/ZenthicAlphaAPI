using Domain.Modularity;
using Identity.Application.Roles.Get;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record GetRoleEndpoint : DefaultGetEndpoint<GetRoleQuery, GetRoleQueryResponse>
{
    public GetRoleEndpoint() : base(Component.Roles) { }
}
