using Application.Validations;
using FluentValidation;
using Identity.Application.Auth.UpdateOAuthUser;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;

namespace Identity.Infrastructure.Auth.UpdateOAuthUser;

internal class UpdateOAuthUserCommandValidator
    : AbstractValidator<UpdateOAuthUserCommand>
{
    public UpdateOAuthUserCommandValidator(IdentityModuleDbContext dbContext)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);

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
            .NotExistIgnoringCurrentAsync(dbContext.OAuthUsers, oauthUser => oauthUser.Email, command => command.Id)
                .WithMessage(GenericValidationErrorMessage.Conflict);
    }
}
