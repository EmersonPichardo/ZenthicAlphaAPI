using Domain.Entities.Implementations;

namespace Identity.Domain.User;

public class UserToken : BaseCompoundEntity
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required string HashingStamp { get; set; }
    public required DateTime Expiration { get; set; }
}
