using Application.Auth;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users.ChangePassword;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.ChangePassword;

internal class ChangeUserPasswordCommandHandler(
    IdentityModuleDbContext dbContext,
    PasswordManager passwordManager,
    IUserSessionService userSessionService
)
    : IRequestHandler<ChangeUserPasswordCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionService.GetSessionAsync();
        var authenticatedSession = (AuthenticatedSession)userSession;

        var foundUser = await dbContext
            .Users
            .SingleOrDefaultAsync(
                user => user.Id == authenticatedSession.Id,
                cancellationToken
            );

        var isInvalidUser = foundUser is null
            || foundUser.Status.HasFlag(UserStatus.Inactive)
            || !passwordManager.Equals(command.CurrentPassword, foundUser.HashedPassword, foundUser.HashingStamp);

        if (foundUser is null || isInvalidUser)
            return FailureFactory.UnauthorizedAccess();

        var passwordResult = passwordManager.Generate(command.NewPassword);

        foundUser.HashedPassword = passwordResult.HashedPassword;
        foundUser.HashingStamp = passwordResult.HashingStamp;
        foundUser.Status = foundUser.Status.RemoveFlag(UserStatus.PasswordChangeRequired);

        dbContext.Users.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}

