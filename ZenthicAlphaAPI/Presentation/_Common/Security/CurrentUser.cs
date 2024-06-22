using Application._Common.Security.Authentication;
using Domain._Common.Modularity;
using Domain.Security;

namespace Presentation._Common.Security;

internal class CurrentUser : ICurrentUser
{
    public required Guid Id { get; init; }
    public required UserStatus Status { get; init; }
    public required IReadOnlyDictionary<Component, int>? Accesses { get; init; }
}