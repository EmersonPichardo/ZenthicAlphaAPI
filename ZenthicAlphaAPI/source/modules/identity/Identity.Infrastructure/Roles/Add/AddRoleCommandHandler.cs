using Application.Events;
using Application.Failures;
using Domain.Modularity;
using Identity.Application._Common.Helpers;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Add;
using Identity.Domain.Roles;
using MediatR;
using OneOf;
using OneOf.Types;
using System.Collections;

namespace Identity.Infrastructure.Roles.Add;

internal class AddRoleCommandHandler(
    IIdentityDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IAddRoleCommandHandler
{
    public async Task<OneOf<None, Failure>> Handle(AddRoleCommand command, CancellationToken cancellationToken)
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

        return new None();
    }
}
