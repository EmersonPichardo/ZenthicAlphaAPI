using Application.Authorization;
using Domain.Modularity;
using FluentValidation;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Update;
using Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Roles.Update;

internal class UpdateRoleCommandValidator
    : AbstractValidator<UpdateRoleCommand>
{
    private readonly int componentsCount = Enum.GetValues(typeof(Component)).Length;
    private readonly int permissionsCount = Enum.GetValues(typeof(Permission)).Length;

    public UpdateRoleCommandValidator(IIdentityDbContext dbContext)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);

        RuleFor(command => command.Name)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(RoleConfiguration.NameLength)
                .WithMessage(GenericValidationErrorMessage.MaximumLength)
            .NotExistIgnoringCurrentAsync(dbContext.Roles, role => role.Name, command => command.Id)
                .WithMessage(GenericValidationErrorMessage.Conflict);

        RuleFor(command => command.SelectedPermissions)
            .Must(value => value.GetLength(0) == componentsCount)
                .WithMessage(GenericValidationErrorMessage.Length.Replace("{MinLength}", $"{componentsCount}"));

        RuleForEach(command => command.SelectedPermissions)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Must(value => value.Length == permissionsCount)
                .WithMessage(GenericValidationErrorMessage.Length.Replace("{MinLength}", $"{permissionsCount}"))
            .ForEach(permission => permission
                .NotNull()
                    .WithMessage(GenericValidationErrorMessage.Required)
            );
    }
}
