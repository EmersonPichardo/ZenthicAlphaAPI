using Application.Validations;
using FluentValidation;
using Identity.Application.Common.Auth;
using Identity.Application.Users.ChangePassword;
using Identity.Application.Validations;

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