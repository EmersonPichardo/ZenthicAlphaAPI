namespace Identity.Application.Common.Auth;

public static class PasswordPolicy
{
    public const int MinimumLength = 8;
    public const string LowercaseRequirement = @"[a-z]+";
    public const string UppercaseRequirement = @"[A-Z]+";
    public const string NumberRequirement = @"[0-9]+";
    public const string SpecialCharacterRequirement = @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]";
}