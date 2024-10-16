using Domain.Entities.Implementations;
using Domain.Modularity;

namespace Identity.Domain.Roles;

public class RolePermission : BaseCompoundEntity
{
    public Guid RoleId { get; set; }
    public required Component Component { get; set; }
    public required Permission RequiredAccess { get; set; }
}
