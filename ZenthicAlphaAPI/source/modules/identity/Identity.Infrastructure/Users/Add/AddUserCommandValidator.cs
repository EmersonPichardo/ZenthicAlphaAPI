using FluentValidation;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.Add;
using Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.Add;

internal class AddUserCommandValidator
    : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator(IIdentityDbContext dbContext)
    {
        RuleFor(command => command.FullName)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required)
            .MaximumLength(UserConfiguration.FullNameLength)
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
