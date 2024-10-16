using Domain.Entities.Implementations;
using Identity.Domain.User;

namespace Identity.Domain.Roles;

public class Role : BaseAuditableCompoundEntity
{
    public required string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> Permissions { get; set; } = [];
}
