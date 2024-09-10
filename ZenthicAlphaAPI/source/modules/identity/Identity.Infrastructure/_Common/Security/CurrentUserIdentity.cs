using Identity.Application._Common.Authentication;

namespace Identity.Infrastructure._Common.Security;

internal class CurrentUserIdentity : ICurrentUserIdentity
{
    public required Guid Id { get; init; }

    public static implicit operator CurrentUserIdentity(Guid id)
        => new() { Id = id };
}