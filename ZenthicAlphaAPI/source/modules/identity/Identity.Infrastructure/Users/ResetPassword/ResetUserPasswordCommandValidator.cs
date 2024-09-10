using FluentValidation;
using Identity.Application.Users.ResetPassword;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.ResetPassword;

internal class ResetUserPasswordCommandValidator
    : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(model => model.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}