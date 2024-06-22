namespace Domain.Security;

public class RolePermission : BaseAuditableCompoundEntity
{
    public Guid RoleId { get; set; }
    public required Component Component { get; set; }
    public required int RequiredAccess { get; set; }

    public Role? Role { get; set; }
}
