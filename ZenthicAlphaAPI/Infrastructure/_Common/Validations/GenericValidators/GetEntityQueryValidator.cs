using Application._Common.Queries;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure._Common.Validations.GenericValidators;

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
