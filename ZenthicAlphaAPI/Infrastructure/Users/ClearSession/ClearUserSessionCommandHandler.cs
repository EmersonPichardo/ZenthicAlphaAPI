using Application._Common.Caching;
using Application._Common.Failures;
using Application._Common.Security.Authentication;
using Application.Users.ClearSession;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Users.ClearSession;

internal class ClearUserSessionCommandHandler(
    ICacheStore cacheStore
)
    : IClearUserSessionCommandHandler
{
    public async Task<OneOf<None, Failure>> Handle(ClearUserSessionCommand command, CancellationToken cancellationToken)
    {
        await cacheStore.ClearAsync(
            $"{nameof(ICurrentUserIdentity)}{{{command.UserId}}}",
            cancellationToken
        );

        return new None();
    }
}
