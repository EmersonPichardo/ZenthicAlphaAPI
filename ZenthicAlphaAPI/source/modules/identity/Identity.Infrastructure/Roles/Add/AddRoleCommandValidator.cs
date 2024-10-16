using Domain.Modularity;
using FluentValidation;
using Identity.Application.Roles.Add;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Roles.Add;

internal class AddRoleCommandValidator
    : AbstractValidator<AddRoleCommand>
{
    private readonly int componentsCount = Enum.GetValues(typeof(Component)).Length;

    public AddRoleCommandValidator(IdentityModuleDbContext dbContext)
    {
        RuleFor(role => role.Name)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(RoleConfiguration.NameLength)
                .WithMessage(GenericValidationErrorMessage.MaximumLength)
            .NotExistAsync(dbContext.Roles, role => role.Name)
                .WithMessage(GenericValidationErrorMessage.Conflict);

        RuleFor(role => role.SelectedPermissions)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Must(permissions => permissions.Count == componentsCount)
                .WithMessage(GenericValidationErrorMessage.Length.Replace("{MinLength}", $"{componentsCount}"));

        RuleForEach(role => role.SelectedPermissions)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Must(value => Enum.TryParse<Component>(value.Key, true, out _))
                .WithMessage(GenericValidationErrorMessage.NotFound);

        RuleForEach(role => role.SelectedPermissions.Values)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .ForEach(access => access
                .NotNull()
                    .WithMessage(GenericValidationErrorMessage.Required)
                .Must(value => Enum.TryParse<Permission>(value, true, out _))
                    .WithMessage(GenericValidationErrorMessage.NotFound)
            );
    }
}