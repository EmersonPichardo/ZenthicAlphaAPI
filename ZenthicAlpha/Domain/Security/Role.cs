namespace Domain.Security;

public class Role : BaseAuditableCompoundEntity
{
    public required string Name { get; set; }

    public IList<RolePermission> Permissions { get; set; } = [];
    public IList<UserRole> UsersRoles { get; set; } = [];
}
