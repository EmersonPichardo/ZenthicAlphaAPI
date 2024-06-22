using Application._Common.Events;
using Application._Common.Exceptions;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application.Users.ResetPassword;
using Domain.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.ResetPassword;

internal class ResetUserPasswordCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
)
    : IResetUserPasswordCommandHandler
{
    public async Task Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                entity => entity.Id.Equals(command.Id),
                cancellationToken
            )
        ?? throw new NotFoundException(nameof(dbContext.Users), command.Id);

        if (foundUser.Status is UserStatus.Inactive)
            throw new InvalidDataException($"{nameof(User)}{{{command.Id}}} is {nameof(UserStatus.Inactive)}.");

        (var newPassword, var hashedPassword, var salt, var algorithm, var iterations)
            = passwordHasher.GenerateNewPassword();

        foundUser.Password = hashedPassword;
        foundUser.Salt = salt;
        foundUser.Algorithm = algorithm;
        foundUser.Iterations = iterations;
        foundUser.Status = UserStatus.Active;

        dbContext.Users.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new UserPasswordResetEvent()
            {
                UserId = foundUser.Id,
                Email = foundUser.Email,
                FullName = foundUser.FullName,
                NewPassword = newPassword
            }
        );
    }
}
