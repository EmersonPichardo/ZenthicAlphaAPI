using Application.Validations;
using FluentValidation;
using Identity.Application.Roles.Delete;

namespace Identity.Infrastructure.Roles.Delete;

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
