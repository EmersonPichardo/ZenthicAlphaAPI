using Application.Validations;
using FluentValidation;
using Identity.Application.Users.ResetPassword;

namespace Identity.Infrastructure.Users.ResetPassword;

internal class ResetUserPasswordCommandValidator
    : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}