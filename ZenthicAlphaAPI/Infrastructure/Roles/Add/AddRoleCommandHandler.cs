using Application._Common.Events;
using Application._Common.Helpers;
using Application._Common.Persistence.Databases;
using Application.Roles.Add;
using Domain._Common.Modularity;
using Domain.Security;
using System.Collections;

namespace Infrastructure.Roles.Add;

internal class AddRoleCommandHandler(
    IApplicationDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IAddRoleCommandHandler
{
    public async Task Handle(AddRoleCommand command, CancellationToken cancellationToken)
    {
        var newRole = new Role
        {
            Name = command.Name
        };

        for (var index = 0; index < command.SelectedPermissions.GetLength(0); index++)
        {
            var rolePermission = new RolePermission()
            {
                Component = (Component)index,
                RequiredAccess = new BitArray(command.SelectedPermissions[index]).ToInt()
            };

            newRole.Permissions.Add(rolePermission);
        }

        dbContext.Roles.Add(newRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleAddedEvent()
            {
                Entity = newRole
            }
        );
    }
}
