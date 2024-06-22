using Application._Common.Security.Authentication;

namespace Presentation._Common.Security;

internal class CurrentUserIdentity : ICurrentUserIdentity
{
    public required Guid Id { get; init; }

    public static implicit operator CurrentUserIdentity(Guid id)
        => new() { Id = id };
}