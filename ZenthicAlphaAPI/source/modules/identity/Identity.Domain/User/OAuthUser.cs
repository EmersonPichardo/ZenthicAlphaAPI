using Domain.Entities.Implementations;

namespace Identity.Domain.User;

public class OAuthUser : BaseAuditableCompoundEntity
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required OAuthUserStatus Status { get; set; }
}
