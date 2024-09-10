using Domain.Modularity;
using Identity.Domain.User;

namespace Identity.Application._Common.Authentication;

public interface ICurrentUser : ICurrentUserIdentity
{
    UserStatus Status { get; init; }
    IReadOnlyDictionary<Component, int>? Accesses { get; init; }
}