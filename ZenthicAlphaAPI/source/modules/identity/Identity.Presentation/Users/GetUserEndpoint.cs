using Domain.Modularity;
using Identity.Application.Users.Get;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record GetUserEndpoint() : DefaultGetEntityEndpoint<GetUserQuery, GetUserQueryResponse>(Component.Users);
