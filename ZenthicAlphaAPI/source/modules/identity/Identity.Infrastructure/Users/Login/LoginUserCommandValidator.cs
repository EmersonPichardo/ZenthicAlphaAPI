﻿using Application.Validations;
using FluentValidation;
using Identity.Application.Users.Login;

namespace Identity.Infrastructure.Users.Login;

internal class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .EmailAddress()
                .WithMessage(GenericValidationErrorMessage.InvalidFormat);

        RuleFor(command => command.Password)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
