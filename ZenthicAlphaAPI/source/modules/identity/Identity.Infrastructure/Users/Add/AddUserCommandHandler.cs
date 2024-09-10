using Application.Events;
using Application.Failures;
using Identity.Application._Common.Authentication;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.Add;
using Identity.Domain.User;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Users.Add;

internal class AddUserCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
)
    : IRequestHandler<AddUserCommand, OneOf<None, Failure>>
{
    public async Task<OneOf<None, Failure>> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        (var newPassword, var hashedPassword, var salt, var algorithm, var iterations)
            = passwordHasher.GenerateNewPassword();

        var user = new User()
        {
            FullName = command.FullName,
            Email = command.Email,
            Password = hashedPassword,
            Salt = salt,
            Algorithm = algorithm,
            Iterations = iterations,
            Status = UserStatus.RequiredPasswordChange,
            UserRoles = command
                .RoleIds
                .Select(roleId => new UserRole() { RoleId = roleId })
                .ToList()
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new UserAddedEvent()
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                NewPassword = newPassword
            }
        );

        return new None();
    }
}
