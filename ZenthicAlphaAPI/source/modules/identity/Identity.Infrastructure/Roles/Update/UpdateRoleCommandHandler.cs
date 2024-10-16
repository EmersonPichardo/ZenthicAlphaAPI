using Application.Events;
using Application.Failures;
using Domain.Modularity;
using Identity.Application.Roles.Update;
using Identity.Domain.Roles;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Identity.Infrastructure.Roles.Update;

internal class UpdateRoleCommandHandler(
    IdentityModuleDbContext dbContext,
    IEventPublisher eventPublisher
)
    : IRequestHandler<UpdateRoleCommand, OneOf<Success, Failure>>
{
    public async Task<OneOf<Success, Failure>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
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

        foreach ((var componentName, var permissionArray) in command.SelectedPermissions)
        {
            var permission = string.Join(',', permissionArray);

            var rolePermission = new RolePermission
            {
                RoleId = foundRole.Id,
                Component = Enum.Parse<Component>(componentName, true),
                RequiredAccess = Enum.Parse<Permission>(permission, true)
            };

            dbContext.RolesPermissions.Add(rolePermission);
        }

        dbContext.Update(foundRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        eventPublisher.EnqueueEvent(
            new RoleUpdatedEvent { Entity = foundRole }
        );

        return new Success();
    }
}
