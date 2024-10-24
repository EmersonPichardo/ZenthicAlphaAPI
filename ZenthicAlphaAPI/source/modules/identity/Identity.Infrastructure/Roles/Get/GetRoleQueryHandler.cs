using Application.Failures;
using Application.Helpers;
using Identity.Application.Roles.Get;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Identity.Infrastructure.Roles.Get;

internal class GetRoleQueryHandler(
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<GetRoleQuery, OneOf<GetRoleQueryResponse, Failure>>
{
    public async Task<OneOf<GetRoleQueryResponse, Failure>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .Include(role => role.Permissions)
            .FirstOrDefaultAsync(
                role => role.Id == request.Id,
                cancellationToken
            );

        if (foundRole is null)
            return FailureFactory.NotFound("Role not found", $"No role was found with an Id of {request.Id}");

        var selectedPermissions = foundRole
            .Permissions
            .OrderBy(permission => permission.Component)
            .ToDictionary(
                permission => permission.Component.ToString(),
                permission => permission.RequiredAccess.AsString()
            );

        return new GetRoleQueryResponse
        {
            Id = request.Id,
            Name = foundRole.Name,
            SelectedPermissions = selectedPermissions
        };
    }
}
