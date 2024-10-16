using Application.Events;

namespace Identity.Application.Users.ResetPassword;

public record UserPasswordResetEvent
    : IEvent
{
    public required UserDto User { get; init; }
    public required string NewPassword { get; init; }
}
