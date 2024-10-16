namespace Identity.Application.Roles.Get;

public record GetRoleQueryResponse
    : RoleDto
{
    public required IReadOnlyDictionary<string, string[]> SelectedPermissions { get; init; }
}