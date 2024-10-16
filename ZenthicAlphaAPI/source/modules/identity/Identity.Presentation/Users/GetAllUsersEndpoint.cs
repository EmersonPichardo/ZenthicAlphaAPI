using Domain.Modularity;
using Identity.Application.Users.GetAll;
using Identity.Application.Users.GetPaginated;
using Presentation.Endpoints.Defaults;

namespace Identity.Presentation.Users;

public record GetAllUsersEndpoint() : DefaultGetListEndpoint<
    GetAllUsersQuery, GetAllUsersQueryResponse,
    GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse
>(Component.Users);
