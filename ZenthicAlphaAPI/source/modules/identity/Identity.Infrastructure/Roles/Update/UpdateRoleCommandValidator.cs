using Domain.Modularity;
using FluentValidation;
using Identity.Application.Roles.Update;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Roles.Update;

internal class UpdateRoleCommandValidator
    : AbstractValidator<UpdateRoleCommand>
{
    private readonly int componentsCount = Enum.GetValues(typeof(Component)).Length;

    public UpdateRoleCommandValidator(IdentityModuleDbContext dbContext)
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

        RuleFor(role => role.SelectedPermissions)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Must(value => value.Count == componentsCount)
                .WithMessage(GenericValidationErrorMessage.Length.Replace("{MinLength}", $"{componentsCount}"));

        RuleForEach(role => role.SelectedPermissions)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Must(value => Enum.TryParse<Component>(value.Key, true, out _))
                .WithMessage(GenericValidationErrorMessage.NotFound);

        RuleForEach(role => role.SelectedPermissions.Values)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .ForEach(permission => permission
                .NotNull()
                    .WithMessage(GenericValidationErrorMessage.Required)
                .Must(value => Enum.TryParse<Permission>(value, true, out _))
                    .WithMessage(GenericValidationErrorMessage.NotFound)
            );
    }
}
