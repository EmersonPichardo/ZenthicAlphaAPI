using FluentValidation;
using Identity.Application.Users.ChangePassword;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Validations.ValidationErrorMessages;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.ChangePassword;

internal class ChangeUserPasswordCommandValidator
    : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(command => command.CurrentPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);

        RuleFor(command => command.NewPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MinimumLength(PasswordPolicy.MinimumLength)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.LowercaseRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.UppercaseRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.NumberRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.SpecialCharacterRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy);

        RuleFor(command => command.RepeatedNewPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Equal(command => command.NewPassword)
                .WithMessage(PasswordValidationErrorMessage.PasswordsMustMatch);
    }
}