﻿using FluentValidation;
using Identity.Application.Users.ChangePassword;
using Identity.Infrastructure._Common.Security;
using Identity.Infrastructure._Common.Validations.ValidationErrorMessages;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.ChangePassword;

internal class ChangeUserPasswordCommandValidator
    : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(model => model.CurrentPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);

        RuleFor(model => model.NewPassword)
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

        RuleFor(model => model.RepeatedNewPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Equal(model => model.NewPassword)
                .WithMessage(PasswordValidationErrorMessage.PasswordsMustMatch);
    }
}