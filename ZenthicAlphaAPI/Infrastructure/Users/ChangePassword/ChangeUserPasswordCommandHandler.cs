using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application._Common.Settings;
using Application.Users.ChangePassword;
using Application.Users.ClearSession;
using Domain.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.ChangePassword;

internal class ChangeUserPasswordCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IIdentityService identityService,
    ISender mediator
)
    : IChangeUserPasswordCommandHandler
{
    public async Task Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = identityService
            .GetCurrentUserIdentity()?
            .Id
        ?? throw new UnauthorizedAccessException();

        var foundUser = await dbContext
            .Users
            .FirstOrDefaultAsync(
                entity
                    => entity.Id.Equals(currentUserId),
                cancellationToken
            )
        ?? throw new UnauthorizedAccessException();

        var hashingSettings = new HashingSettings()
        {
            Algorithm = foundUser.Algorithm,
            Iterations = foundUser.Iterations
        };

        var hashedPassword = passwordHasher.Hash(
            command.CurrentPassword, foundUser.Salt, hashingSettings
        );

        if (!hashedPassword.Equals(foundUser.Password, StringComparison.Ordinal))
            throw new UnauthorizedAccessException();

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
    }
}

