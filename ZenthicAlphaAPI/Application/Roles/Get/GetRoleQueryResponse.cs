namespace Application.Roles.Get;

public record GetRoleQueryResponse
    : RoleDto
{
    public required bool[][] SelectedPermissions { get; init; }
}