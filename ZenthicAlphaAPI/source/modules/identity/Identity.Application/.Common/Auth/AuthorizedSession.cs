using Application.Auth;
using Domain.Modularity;

namespace Identity.Application.Common.Auth;

public record AuthorizedSession : AuthenticatedSession
{
    public required IReadOnlyDictionary<Component, Permission> Accesses { get; init; }
}
