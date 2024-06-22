using Application._Common.Caching;
using Application._Common.Security.Authentication;
using Application.Users.ClearSession;

namespace Infrastructure.Users.ClearSession;

internal class ClearUserSessionCommandHandler(
    ICacheStore cacheStore
)
    : IClearUserSessionCommandHandler
{
    public async Task Handle(ClearUserSessionCommand command, CancellationToken cancellationToken)
    {
        await cacheStore.ClearAsync(
            $"{nameof(ICurrentUserIdentity)}{{{command.UserId}}}",
            cancellationToken
        );
    }
}
