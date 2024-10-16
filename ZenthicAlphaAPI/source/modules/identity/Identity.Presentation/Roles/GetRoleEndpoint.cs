using Domain.Modularity;
using Identity.Application.Roles.Get;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Roles;

public record GetRoleEndpoint() : DefaultGetEntityEndpoint<GetRoleQuery, GetRoleQueryResponse>(Component.Roles);
