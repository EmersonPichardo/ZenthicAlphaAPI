using Application.Events;
using Application.Failures;
using Domain.Modularity;
using Identity.Application._Common.Helpers;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Update;
using Identity.Domain.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using System.Collections;

namespace Identity.Infrastructure.Roles.Update;

internal class UpdateRoleCommandHandler(
    IIdentityDbContext dbContext,
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
