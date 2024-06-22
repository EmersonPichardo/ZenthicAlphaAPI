using Application._Common.Persistence.Databases;
using Application._Common.Security.Authorization;
using Application.Roles.Add;
using Domain._Common.Modularity;
using FluentValidation;
using Infrastructure._Common.Validations;
using Infrastructure._Common.Validations.ValidationErrorMessages;
using Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;

namespace Infrastructure.Roles.Add;

internal class AddRoleCommandValidator
    : AbstractValidator<AddRoleCommand>
{
    private readonly int componentsCount = Enum.GetValues(typeof(Component)).Length;
    private readonly int permissionsCount = Enum.GetValues(typeof(Permission)).Length;

    public AddRoleCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(role => role.Name)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(RoleConfiguration.NameLength)
                .WithMessage(GenericValidationErrorMessage.MaximumLength)
            .NotExistAsync(dbContext.Roles, role => role.Name)
                .WithMessage(GenericValidationErrorMessage.Conflict);

        RuleFor(role => role.SelectedPermissions)
            .Must(value => value.GetLength(0) == componentsCount)
                .WithMessage(GenericValidationErrorMessage.Length.Replace("{MinLength}", $"{componentsCount}"));

        RuleForEach(role => role.SelectedPermissions)
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
