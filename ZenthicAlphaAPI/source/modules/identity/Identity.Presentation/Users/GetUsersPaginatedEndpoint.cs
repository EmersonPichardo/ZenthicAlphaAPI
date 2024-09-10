using Domain.Modularity;
using Identity.Application.Users.GetPaginated;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record GetUsersPaginatedEndpoint : DefaultGetPaginatedEndpoint<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse>
{
    public GetUsersPaginatedEndpoint() : base(Component.Users) { }
}
