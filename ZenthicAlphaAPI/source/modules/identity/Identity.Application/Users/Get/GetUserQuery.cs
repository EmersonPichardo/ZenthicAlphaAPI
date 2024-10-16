using Application.Auth;
using Application.Caching;
using Application.Queries;
using Domain.Modularity;

namespace Identity.Application.Users.Get;

[Cache(Component.Users)]
[Authorize(Component.Users, Permission.Read)]
public record GetUserQuery
    : GetEntityQuery<GetUserQueryResponse>;
