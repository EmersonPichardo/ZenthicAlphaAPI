using Domain.Modularity;
using Identity.Application.Roles.Delete;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record DeleteRoleEndpoint : DefaultDeleteEndpoint<DeleteRoleCommand>
{
    public DeleteRoleEndpoint() : base(Component.Roles) { }
}
