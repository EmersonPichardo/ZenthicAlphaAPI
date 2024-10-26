using Application.Validations;
using FluentValidation;
using Identity.Application.Auth.AddOAuthUser;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;

namespace Identity.Infrastructure.Auth.AddOAuthUser;

internal class AddOAuthUserCommandValidator
    : AbstractValidator<AddOAuthUserCommand>
{
    public AddOAuthUserCommandValidator(IdentityModuleDbContext dbContext)
    {
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
            .NotExistAsync(dbContext.OAuthUsers, entity => entity.Email)
                .WithMessage(GenericValidationErrorMessage.Conflict);

    }
}
