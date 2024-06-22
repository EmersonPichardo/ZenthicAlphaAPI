using Application._Common.Security.Authentication;
using Application.Users.ClearSession;
using Application.Users.Logout;
using MediatR;

namespace Infrastructure.Users.Logout;

internal class LogoutCurrentUserCommandHandler(
    IIdentityService IdentityService,
    ISender mediator
)
    : ILogoutCurrentUserCommandHandler
{
    public async Task Handle(LogoutCurrentUserCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = IdentityService.GetCurrentUserIdentity()?.Id;

        if (currentUserId is null)
            return;

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = currentUserId.Value },
            cancellationToken
        );
    }
}
