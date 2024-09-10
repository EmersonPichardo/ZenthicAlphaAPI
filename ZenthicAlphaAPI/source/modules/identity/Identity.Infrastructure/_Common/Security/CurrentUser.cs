using Domain.Modularity;
using Identity.Application._Common.Authentication;
using Identity.Domain.User;

namespace Identity.Infrastructure._Common.Security;

internal class CurrentUser : ICurrentUser
{
    public required Guid Id { get; init; }
    public required UserStatus Status { get; init; }
    public required IReadOnlyDictionary<Component, int>? Accesses { get; init; }
}