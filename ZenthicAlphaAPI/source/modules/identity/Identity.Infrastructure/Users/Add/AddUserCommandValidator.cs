using FluentValidation;
using Identity.Application.Users.Add;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Validations.ValidationErrorMessages;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.Add;

internal class AddUserCommandValidator
    : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator(IdentityModuleDbContext dbContext)
    {
        RuleFor(model => model.Password)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MinimumLength(PasswordPolicy.MinimumLength)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.LowercaseRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.UppercaseRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.NumberRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy)
            .Matches(PasswordPolicy.SpecialCharacterRequirement)
                .WithMessage(PasswordValidationErrorMessage.IncorrectPasswordPolicy);

        RuleFor(model => model.RepeatedPassword)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .Equal(model => model.Password)
                .WithMessage(PasswordValidationErrorMessage.PasswordsMustMatch);

        RuleFor(command => command.UserName)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(UserConfiguration.UserNameLength)
                .WithMessage(GenericValidationErrorMessage.MaximumLength);

        RuleFor(command => command.Email)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(UserConfiguration.EmailLength)
                .WithMessage(GenericValidationErrorMessage.MaximumLength)
            .EmailAddress()
                .WithMessage(GenericValidationErrorMessage.InvalidFormat)
            .NotExistAsync(dbContext.Users, entity => entity.Email)
                .WithMessage(GenericValidationErrorMessage.Conflict);

        RuleForEach(model => model.RoleIds)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .ExistAsync(dbContext.Roles, entity => entity.Id)
                .WithMessage(GenericValidationErrorMessage.NotFound);
    }
}
