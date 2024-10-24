using FluentValidation;
using Identity.Application.Users.ConfirmEmail;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.ConfirmEmail;

internal class ConfirmEmailCommandValidator
    : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
