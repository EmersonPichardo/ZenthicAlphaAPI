using Application.Caching;
using Application.Failures;
using Identity.Application._Common.Authentication;
using Identity.Application.Users.ClearSession;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.ClearSession;

internal class ClearUserSessionCommandHandler(
    ICacheStore cacheStore
)
    : IRequestHandler<ClearUserSessionCommand, OneOf<None, Failure>>
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
