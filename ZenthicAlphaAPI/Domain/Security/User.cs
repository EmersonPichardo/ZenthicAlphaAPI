namespace Domain.Security;

public class User : BaseAuditableCompoundEntity
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public required string Algorithm { get; set; }
    public required short Iterations { get; set; }
    public required UserStatus Status { get; set; }

    public IList<UserRole> UserRoles { get; set; } = [];
}

public enum UserStatus
{
    Inactive = 0,
    Active = 1,
    RequiredPasswordChange = 2
}