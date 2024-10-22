using FluentValidation;
using Identity.Application.OAuth.OAuthCallback;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Identity.Infrastructure.Auth.OAuthCallback;

internal class OAuthCallbackCommandValidator
    : AbstractValidator<OAuthCallbackCommand>
{
    public OAuthCallbackCommandValidator()
    {
        RuleFor(model => model.AuthenticationScheme)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
