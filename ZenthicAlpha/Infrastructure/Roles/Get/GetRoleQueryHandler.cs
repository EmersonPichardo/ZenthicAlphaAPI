using Application._Common.Exceptions;
using Application._Common.Helpers;
using Application._Common.Persistence.Databases;
using Application.Roles.Get;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Roles.Get;

internal class GetRoleQueryHandler(
    IApplicationDbContext dbContext
)
    : IGetRoleQueryHandler
{
    public async Task<GetRoleQueryResponse> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var foundRole = await dbContext
            .Roles
            .Include(role => role.Permissions)
            .FirstOrDefaultAsync(
                role => role.Id.Equals(request.Id),
                cancellationToken
            )
        ?? throw new NotFoundException(nameof(dbContext.Roles), request.Id);

        var selectedPermissions = foundRole
            .Permissions
            .OrderBy(permission => permission.Component)
            .GroupBy(
                permission => permission.Component,
                permission => permission.RequiredAccess.ToBoolArray(),
                (_, group) => group.First()
            )
            .ToArray();

        return new()
        {
            Id = request.Id,
            Name = foundRole.Name,
            SelectedPermissions = selectedPermissions
        };
    }
}
