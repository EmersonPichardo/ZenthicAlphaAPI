using Application.Users.Login;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure.Users.Login;

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
