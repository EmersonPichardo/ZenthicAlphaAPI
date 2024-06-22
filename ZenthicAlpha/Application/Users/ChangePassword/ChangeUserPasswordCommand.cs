namespace Application.Users.ChangePassword;

public record ChangeUserPasswordCommand
    : ICommand
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
    public required string RepeatedNewPassword { get; init; }
}