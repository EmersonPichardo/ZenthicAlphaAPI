using Domain.Modularity;
using Identity.Application.Roles.Update;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record UpdateRoleEndpoint : DefaultUpdateEndpoint<UpdateRoleCommand>
{
    public UpdateRoleEndpoint() : base(Component.Roles) { }
}
