﻿using Application.Auth;
using Application.Validations;
using FluentValidation;
using Identity.Application.Users.Update;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;
using Infrastructure.Validations;

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
                        var userSession = context.RootContextData[nameof(IUserSession)];
                        var authenticatedSession = (AuthenticatedSession)userSession;

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
