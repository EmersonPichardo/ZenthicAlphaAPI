using Application.Users.ClearSession;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure.Users.ClearSession;

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
