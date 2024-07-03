using Application._Common.Events;
using Application._Common.Failures;
using Application._Common.Helpers;
using Application._Common.Persistence.Databases;
using Application.Roles.Update;
using Domain._Common.Modularity;
using Domain.Security;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using System.Collections;

namespace Infrastructure.Roles.Update;

internal class UpdateRoleCommandHandler(
    IApplicationDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IUpdateRoleCommandHandler
{
    public async Task<OneOf<None, Failure>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .FindAsync([command.Id], cancellationToken);

        if (foundRole is null)
            return FailureFactory.NotFound("Role not found", $"No role was found with an Id of {command.Id}");

        foundRole.Name = command.Name;

        await dbContext
            .RolesPermissions
            .Where(entity => entity.RoleId.Equals(command.Id))
            .ExecuteDeleteAsync(cancellationToken);

        for (var index = 0; index < command.SelectedPermissions.GetLength(0); index++)
        {
            var rolePermission = new RolePermission()
            {
                Component = (Component)index,
                RequiredAccess = new BitArray(command.SelectedPermissions[index]).ToInt()
            };

            foundRole.Permissions.Add(rolePermission);
        }

        dbContext.Roles.Update(foundRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleUpdatedEvent() { Entity = foundRole }
        );

        return new None();
    }
}
