using Domain.Entities.Implementations;
using Identity.Domain.Roles;

namespace Identity.Domain.User;

public class OAuthUserRole : BaseCompoundEntity
{
    public Guid OAuthUserId { get; set; }
    public Guid RoleId { get; set; }

    public OAuthUser OAuthUser { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
