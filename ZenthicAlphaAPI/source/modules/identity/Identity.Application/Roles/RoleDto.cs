namespace Identity.Application.Roles;

public record RoleDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}