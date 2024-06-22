using Infrastructure._Common.Security;

namespace Infrastructure._Common.Validations.ValidationErrorMessages;

internal static class PasswordValidationErrorMessage
{
    internal const string MustContainLowercase = "Must contain at least one lowercase letter";
    internal const string MustContainUppercase = "Must contain at least one uppercase letter";
    internal const string MustContainNumbers = "Must contain at least one number";
    internal const string MustContainSpecials = "Must contain at least one special character (!@#$%^&*()_+=[]{};:<>|./?,-)";
    internal const string PasswordsMustMatch = "Passwords are not the same";
    internal static readonly string IncorrectPasswordPolicy =
        @$"{GenericValidationErrorMessage.MinimumLength.Replace("{MinLength}", PasswordPolicy.MinimumLength.ToString())}.
        {MustContainUppercase}.
        {MustContainUppercase}.
        {MustContainNumbers}.
        MustContainSpecials";
}
