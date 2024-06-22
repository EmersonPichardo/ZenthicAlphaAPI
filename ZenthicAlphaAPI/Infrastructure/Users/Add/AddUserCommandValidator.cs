using Application._Common.Persistence.Databases;
using Application.Users.Add;
using FluentValidation;
using Infrastructure._Common.Validations;
using Infrastructure._Common.Validations.ValidationErrorMessages;
using Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;

namespace Infrastructure.Users.Add;

internal class AddUserCommandValidator
    : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator(IApplicationDbContext dbContext)
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
