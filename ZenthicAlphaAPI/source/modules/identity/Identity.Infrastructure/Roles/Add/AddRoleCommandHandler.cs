using Application.Events;
using Application.Failures;
using Domain.Modularity;
using Identity.Application.Roles.Add;
using Identity.Domain.Roles;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Roles.Add;

internal class AddRoleCommandHandler(
    IdentityModuleDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IRequestHandler<AddRoleCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(AddRoleCommand command, CancellationToken cancellationToken)
    {
        var newRole = new Role
        {
            Name = command.Name
        };

        foreach ((var componentName, var permissionArray) in command.SelectedPermissions)
        {
            var permission = string.Join(',', permissionArray);

            var rolePermission = new RolePermission
            {
                Component = Enum.Parse<Component>(componentName, true),
                RequiredAccess = Enum.Parse<Permission>(permission, true)
            };

            newRole.Permissions.Add(rolePermission);
        }

        dbContext.Roles.Add(newRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleAddedEvent { Entity = newRole }
        );

        return new Success();
    }
}
