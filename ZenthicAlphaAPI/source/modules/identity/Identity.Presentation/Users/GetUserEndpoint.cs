using Domain.Modularity;
using Identity.Application.Users.Get;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record GetUserEndpoint : DefaultGetEndpoint<GetUserQuery, GetUserQueryResponse>
{
    public GetUserEndpoint() : base(Component.Users) { }
}
