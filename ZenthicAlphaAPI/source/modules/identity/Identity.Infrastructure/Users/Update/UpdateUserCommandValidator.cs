using Application.Auth;
using FluentValidation;
using Identity.Application.Users.Update;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Users.Update;

internal class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(
        IdentityModuleDbContext dbContext)
    {
        RuleFor(command => command)
            .Authenticated()
                .WithMessage(GenericValidationErrorMessage.Unauthorized)
            .DependentRules(() =>
            {
                RuleFor(command => command.Email)
                    .Custom((_, context) =>
                    {
                        var authenticatedSession = (AuthenticatedSession)context.RootContextData[nameof(IUserSession)];

                        RuleFor(command => command.Email)
                            .NotExistIgnoringCurrentAsync(dbContext.Users, user => user.Email, command => authenticatedSession.Id)
                                .WithMessage(GenericValidationErrorMessage.Conflict);
                    });
            });

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
                .WithMessage(GenericValidationErrorMessage.InvalidFormat);
    }
}
