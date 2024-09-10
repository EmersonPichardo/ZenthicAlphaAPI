using Domain.Entities.Implementations;
using Identity.Domain.User;

namespace Identity.Domain.Roles;

public class Role : BaseAuditableCompoundEntity
{
    public required string Name { get; set; }

    public IList<RolePermission> Permissions { get; set; } = [];
    public IList<UserRole> UsersRoles { get; set; } = [];
}
