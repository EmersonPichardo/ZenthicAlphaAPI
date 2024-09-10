using Application.Mapping;
using Identity.Domain.Roles;

namespace Identity.Application.Roles;

public record RoleDto : IMapFrom<Role>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}