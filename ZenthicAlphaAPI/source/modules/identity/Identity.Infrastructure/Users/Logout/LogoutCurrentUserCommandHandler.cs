using Application.Failures;
using Application.Helpers;
using Identity.Application._Common.Authentication;
using Identity.Application.Users.ClearSession;
using Identity.Application.Users.Logout;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.Logout;

internal class LogoutCurrentUserCommandHandler(
    IIdentityService identityService,
    ISender mediator
)
    : IRequestHandler<LogoutCurrentUserCommand, OneOf<None, Failure>>
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
