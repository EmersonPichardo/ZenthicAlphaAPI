using Application.Queries;
using FluentValidation;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Infrastructure.Validations.GenericValidators;

internal abstract class GetEntityQueryValidator<TQuery>
    : AbstractValidator<TQuery>
    where TQuery : IGetEntityQuery
{
    protected GetEntityQueryValidator()
    {
        RuleFor(model => model.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
