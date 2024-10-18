using Application.Events;
using Application.Failures;
using Domain.Identity;
using Identity.Application.Users;
using Identity.Application.Users.Add;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Users.Add;

internal class AddUserCommandHandler(
    IdentityModuleDbContext dbContext,
    PasswordManager passwordManager,
    IEventPublisher eventPublisher
)
    : IRequestHandler<AddUserCommand, OneOf<AddUserCommandResponse, Failure>>
{
    public async Task<OneOf<AddUserCommandResponse, Failure>> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        var newPasswordResult = passwordManager.Generate(command.Password);

        var user = new User
        {
            UserName = command.UserName,
            Email = command.Email,
            HashedPassword = newPasswordResult.HashedPassword,
            HashingStamp = newPasswordResult.HashingStamp,
            Status = UserStatus.UnconfirmEmail,
            UserRoles = command
                .RoleIds
                .Select(roleId => new UserRole { RoleId = roleId })
                .ToList()
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new UserAddedEvent { User = UserDto.FromUser(user) }
        );

        return new AddUserCommandResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Status = user.Status.ToString()
        };
    }
}
