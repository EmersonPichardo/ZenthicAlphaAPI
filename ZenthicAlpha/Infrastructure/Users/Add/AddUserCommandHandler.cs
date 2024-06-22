using Application._Common.Events;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application.Users.Add;
using Domain.Security;

namespace Infrastructure.Users.Add;

internal class AddUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
)
    : IAddUserCommandHandler
{
    public async Task Handle(AddUserCommand command, CancellationToken cancellationToken)
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
    }
}
