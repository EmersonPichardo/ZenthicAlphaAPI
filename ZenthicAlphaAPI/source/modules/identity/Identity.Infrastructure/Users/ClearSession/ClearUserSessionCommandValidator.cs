using FluentValidation;
using Identity.Application.Users.ClearSession;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.ClearSession;

internal class ClearUserSessionCommandValidator
    : AbstractValidator<ClearUserSessionCommand>
{
    public ClearUserSessionCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
