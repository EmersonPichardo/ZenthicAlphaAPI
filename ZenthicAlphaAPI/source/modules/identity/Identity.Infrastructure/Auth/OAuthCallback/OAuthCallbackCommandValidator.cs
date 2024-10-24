using FluentValidation;
using Identity.Application.Auth.OAuthCallback;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Auth.OAuthCallback;

internal class OAuthCallbackCommandValidator
    : AbstractValidator<OAuthCallbackCommand>
{
    public OAuthCallbackCommandValidator()
    {
        RuleFor(query => query.AuthenticationScheme)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
