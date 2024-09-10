using Identity.Application.Roles;

namespace Identity.Application.Roles.Get;

public record GetRoleQueryResponse
    : RoleDto
{
    public required bool[][] SelectedPermissions { get; init; }
}