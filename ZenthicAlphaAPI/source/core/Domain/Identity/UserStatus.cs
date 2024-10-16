namespace Domain.Identity;

[Flags]
public enum UserStatus
{
    None = 0,
    Inactive = 1,
    PasswordChangeRequired = 2,
    UnconfirmEmail = 4
}