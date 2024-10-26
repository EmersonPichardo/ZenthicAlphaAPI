using Application.Validations;
using FluentValidation;
using Identity.Application.Common.Auth;
using Identity.Application.Users.Add;
using Identity.Application.Validations;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;

namespace Identity.Infrastructure.Users.Add;

internal class AddUserCommandValidator
    : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator(IdentityModuleDbContext dbContext)
    {
        RuleFor(command => command.Password)
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

        RuleFor(command => command.RepeatedPassword)
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

        RuleForEach(command => command.RoleIds)
            .ExistAsync(dbContext.Roles, entity => entity.Id)
                .WithMessage(GenericValidationErrorMessage.NotFound);
    }
}
