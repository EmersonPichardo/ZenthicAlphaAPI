using Application.Authorization;
using Domain.Modularity;
using FluentValidation;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.Add;
using Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Roles.Add;

internal class AddRoleCommandValidator
    : AbstractValidator<AddRoleCommand>
{
    private readonly int componentsCount = Enum.GetValues(typeof(Component)).Length;
    private readonly int permissionsCount = Enum.GetValues(typeof(Permission)).Length;

    public AddRoleCommandValidator(IIdentityDbContext dbContext)
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
