using Domain._Common.Modularity;
using Domain.Security;

namespace Application._Common.Security.Authentication;

public interface ICurrentUser : ICurrentUserIdentity
{
    UserStatus Status { get; init; }
    IReadOnlyDictionary<Component, int>? Accesses { get; init; }
}