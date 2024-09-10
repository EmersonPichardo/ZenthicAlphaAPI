using Application.Failures;
using Identity.Application._Common.Helpers;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Get;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Identity.Infrastructure.Roles.Get;

internal class GetRoleQueryHandler(
    IIdentityDbContext dbContext
)
    : IRequestHandler<GetRoleQuery, OneOf<GetRoleQueryResponse, Failure>>
{
    public async Task<OneOf<GetRoleQueryResponse, Failure>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .Include(role => role.Permissions)
            .FirstOrDefaultAsync(
                role => role.Id.Equals(request.Id),
                cancellationToken
            );

        if (foundRole is null)
            return FailureFactory.NotFound("Role not found", $"No role was found with an Id of {request.Id}");

        var selectedPermissions = foundRole
            .Permissions
            .OrderBy(permission => permission.Component)
            .GroupBy(
                permission => permission.Component,
                permission => permission.RequiredAccess.ToBoolArray(),
                (_, group) => group.First()
            )
            .ToArray();

        return new GetRoleQueryResponse()
        {
            Id = request.Id,
            Name = foundRole.Name,
            SelectedPermissions = selectedPermissions
        };
    }
}
