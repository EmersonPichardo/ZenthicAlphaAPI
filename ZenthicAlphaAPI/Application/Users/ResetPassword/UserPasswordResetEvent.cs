namespace Application.Users.ResetPassword;

public record UserPasswordResetEvent
    : IEvent
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string NewPassword { get; init; }
}
