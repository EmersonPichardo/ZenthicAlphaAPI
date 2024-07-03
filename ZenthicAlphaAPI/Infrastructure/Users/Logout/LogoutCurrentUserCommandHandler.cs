using Application._Common.Failures;
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

        if (currentUserIdentityResult.IsT1)
            return new None();

        if (currentUserIdentityResult.IsT2)
            return currentUserIdentityResult.AsT2;

        var currentUserId = currentUserIdentityResult.AsT0.Id;

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = currentUserId },
            cancellationToken
        );

        return new None();
    }
}
