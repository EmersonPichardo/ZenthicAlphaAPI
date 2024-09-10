using Application.Failures;
using Application.Helpers;
using Identity.Application._Common.Authentication;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application._Common.Settings;
using Identity.Application.Users.ChangePassword;
using Identity.Application.Users.ClearSession;
using Identity.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.ChangePassword;

internal class ChangeUserPasswordCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    IIdentityService identityService,
    ISender mediator
)
    : IRequestHandler<ChangeUserPasswordCommand, OneOf<None, Failure>>
{
    public async Task<OneOf<None, Failure>> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var currentUserIdentityResult = identityService
            .GetCurrentUserIdentity();

        if (currentUserIdentityResult.IsNull())
            return new None();

        if (currentUserIdentityResult.IsFailure())
            return currentUserIdentityResult.GetValueAsFailure();

        var currentUserId = currentUserIdentityResult.GetValueAs<ICurrentUserIdentity>().Id;

        var foundUser = await dbContext
            .Users
            .FirstOrDefaultAsync(
                entity
                    => entity.Id.Equals(currentUserId),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.UnauthorizedAccess();

        var hashingSettings = new HashingSettings()
        {
            Algorithm = foundUser.Algorithm,
            Iterations = foundUser.Iterations
        };
        var hashedPassword = passwordHasher.Hash(
            command.CurrentPassword, foundUser.Salt, hashingSettings
        );

        if (!hashedPassword.Equals(foundUser.Password, StringComparison.Ordinal))
            return FailureFactory.UnauthorizedAccess("", "");

        (var hashedNewPassword, var salt, var algorithm, var iterations)
            = passwordHasher.Generate(command.NewPassword);

        foundUser.Password = hashedNewPassword;
        foundUser.Salt = salt;
        foundUser.Algorithm = algorithm;
        foundUser.Iterations = iterations;
        foundUser.Status = UserStatus.Active;

        dbContext.Users.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = foundUser.Id },
            cancellationToken
        );

        return new None();
    }
}

