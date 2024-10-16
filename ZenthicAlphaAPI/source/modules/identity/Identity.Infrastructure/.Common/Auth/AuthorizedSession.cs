using Application.Auth;
using Domain.Modularity;

namespace Identity.Infrastructure.Common.Auth;

internal record AuthorizedSession : AuthenticatedSession
{
    internal required IReadOnlyDictionary<Component, Permission> Accesses { get; init; }
}
