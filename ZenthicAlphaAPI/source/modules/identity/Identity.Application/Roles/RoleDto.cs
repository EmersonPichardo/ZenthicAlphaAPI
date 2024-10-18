using Identity.Domain.Roles;

namespace Identity.Application.Roles;

public record RoleDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public static RoleDto FromRole(Role role) => new()
    {
        Id = role.Id,
        Name = role.Name,
    };
}