using Application.Events;
using Application.Failures;
using Application.Helpers;
using Domain.Identity;
using Identity.Application.Users;
using Identity.Application.Users.ResetPassword;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.ResetPassword;

internal class ResetUserPasswordCommandHandler(
    IdentityModuleDbContext dbContext,
    PasswordManager passwordManager,
    IEventPublisher eventPublisher
)
    : IRequestHandler<ResetUserPasswordCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .SingleOrDefaultAsync(
                user => user.Id.Equals(command.Id),
                cancellationToken
            );

        var isInvalidUser = foundUser is null
            || foundUser.Status.HasFlag(UserStatus.Inactive);

        if (foundUser is null || isInvalidUser)
            return FailureFactory.NotFound("Usuario no encontrado", $"No user was found with an Id of {command.Id}");

        if (foundUser.Status is UserStatus.Inactive)
            return FailureFactory.InvalidRequest("Usuario inactivo", $"{foundUser.UserName} se encuentra inactivo");

        var newPasswordResult = passwordManager.Generate();

        foundUser.HashedPassword = newPasswordResult.HashedPassword;
        foundUser.HashingStamp = newPasswordResult.HashingStamp;
        foundUser.Status = foundUser.Status.AddFlag(UserStatus.PasswordChangeRequired);

        dbContext.Users.Update(foundUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new UserPasswordResetEvent
            {
                User = UserDto.FromUser(foundUser),
                NewPassword = newPasswordResult.NewPassword
            }
        );

        return new Success();
    }
}
