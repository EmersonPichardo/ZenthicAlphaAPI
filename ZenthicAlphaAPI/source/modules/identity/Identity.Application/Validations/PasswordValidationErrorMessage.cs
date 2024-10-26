using Application.Validations;
using Identity.Application.Common.Auth;

namespace Identity.Application.Validations;

public static class PasswordValidationErrorMessage
{
    public const string MustContainLowercase = "Must contain at least one lowercase letter";
    public const string MustContainUppercase = "Must contain at least one uppercase letter";
    public const string MustContainNumbers = "Must contain at least one number";
    public const string MustContainSpecials = "Must contain at least one special character (!@#$%^&*()_+=[]{};:<>|./?,-)";
    public const string PasswordsMustMatch = "Passwords are not the same";
    public static readonly string IncorrectPasswordPolicy =
        @$"{GenericValidationErrorMessage.MinimumLength.Replace("{MinLength}", PasswordPolicy.MinimumLength.ToString())}.
        {MustContainUppercase}.
        {MustContainUppercase}.
        {MustContainNumbers}.
        MustContainSpecials";
}
