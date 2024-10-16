using Domain.Entities.Implementations;
using Domain.Identity;

namespace Identity.Domain.User;

public class User : BaseAuditableCompoundEntity
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    public required string HashingStamp { get; set; }
    public required UserStatus Status { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserToken> Tokens { get; set; } = [];
}