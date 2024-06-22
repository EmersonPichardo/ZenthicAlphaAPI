using Application.Roles.Delete;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure.Roles.Delete;

internal class DeleteRoleCommandValidator
    : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
