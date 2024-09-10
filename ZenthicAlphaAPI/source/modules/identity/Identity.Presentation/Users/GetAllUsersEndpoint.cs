using Domain.Modularity;
using Identity.Application.Users.GetAll;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record GetAllUsersEndpoint : DefaultGetAllEndpoint<GetAllUsersQuery, GetAllUsersQueryResponse>
{
    public GetAllUsersEndpoint() : base(Component.Users) { }
}
