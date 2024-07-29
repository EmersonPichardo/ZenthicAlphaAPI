using Application._Common.Events;
using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application.Users.ResetPassword;
using Domain.Security;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Users.ResetPassword;

internal class ResetUserPasswordCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
)
    : IResetUserPasswordCommandHandler
{
    public async Task<OneOf<None, Failure>> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .FindAsync([command.Id], cancellationToken);

        if (foundUser is null)
            return FailureFactory.NotFound("Usuario no encontrado", $"No user was found with an Id of {command.Id}");

        if (foundUser.Status is UserStatus.Inactive)
            return FailureFactory.Generic("Usuario inactivo", $"{foundUser.FullName} se encuentra inactivo");

        (var newPassword, var hashedPassword, var salt, var algorithm, var iterations)
            = passwordHasher.GenerateNewPassword();

        foundUser.Password = hashedPassword;
        foundUser.Salt = salt;
        foundUser.Algorithm = algorithm;
        foundUser.Iterations = iterations;
        foundUser.Status = UserStatus.RequiredPasswordChange;

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

        return new None();
    }
}
