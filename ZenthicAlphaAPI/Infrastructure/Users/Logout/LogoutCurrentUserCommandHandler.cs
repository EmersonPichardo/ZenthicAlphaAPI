using Application._Common.Failures;
using Application._Common.Helpers;
using Application._Common.Security.Authentication;
using Application.Users.ClearSession;
using Application.Users.Logout;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Users.Logout;

internal class LogoutCurrentUserCommandHandler(
    IIdentityService identityService,
    ISender mediator
)
    : ILogoutCurrentUserCommandHandler
{
    public async Task<OneOf<None, Failure>> Handle(LogoutCurrentUserCommand command, CancellationToken cancellationToken)
    {
        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        if (currentUserIdentityResult.IsNull())
            return new None();

        if (currentUserIdentityResult.IsFailure())
            return currentUserIdentityResult.GetValueAsFailure();

        var currentUserId = currentUserIdentityResult.GetValueAs<ICurrentUserIdentity>().Id;

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = currentUserId },
            cancellationToken
        );

        return new None();
    }
}
