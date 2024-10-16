namespace Identity.Infrastructure.Common.Auth;

internal static class PasswordPolicy
{
    internal const int MinimumLength = 8;
    internal const string LowercaseRequirement = @"[a-z]+";
    internal const string UppercaseRequirement = @"[A-Z]+";
    internal const string NumberRequirement = @"[0-9]+";
    internal const string SpecialCharacterRequirement = @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]";
}