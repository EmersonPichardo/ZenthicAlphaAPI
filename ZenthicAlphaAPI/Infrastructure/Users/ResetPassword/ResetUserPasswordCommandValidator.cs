using Application.Users.ResetPassword;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure.Users.ResetPassword;

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