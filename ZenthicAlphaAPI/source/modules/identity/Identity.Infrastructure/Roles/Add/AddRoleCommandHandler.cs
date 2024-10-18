using Application.Failures;
using Domain.Modularity;
using Identity.Application.Roles.Add;
using Identity.Domain.Roles;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Roles.Add;

internal class AddRoleCommandHandler(
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<AddRoleCommand, OneOf<AddRoleCommandResponse, Failure>>
{
    public async Task<OneOf<AddRoleCommandResponse, Failure>> Handle(AddRoleCommand command, CancellationToken cancellationToken)
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

        return new AddRoleCommandResponse
        {
            Id = newRole.Id,
            Name = newRole.Name
        };
    }
}
