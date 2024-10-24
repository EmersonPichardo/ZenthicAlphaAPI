using Domain.Entities.Implementations;

namespace Identity.Domain.User;

public class OAuthUser : BaseCompoundEntity
{
    public required string AuthenticationType { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required OAuthUserStatus Status { get; set; }

    public ICollection<OAuthUserRole> OAuthUserRoles { get; set; } = [];
}
