using Application._Common.Pagination;
using FluentValidation;
using Infrastructure._Common.Validations.ValidationErrorMessages;

namespace Infrastructure._Common.Validations.GenericValidators;

internal abstract class GetEntitiesPaginatedQueryValidator<TQuery>
    : AbstractValidator<TQuery>
    where TQuery : IGetEntitiesPaginatedQuery
{
    protected GetEntitiesPaginatedQueryValidator()
    {
        RuleFor(model => model.CurrentPage)
            .GreaterThan(0)
                .WithMessage(GenericValidationErrorMessage.GreaterThan);

        RuleFor(model => model.PageSize)
            .GreaterThan(0)
                .WithMessage(GenericValidationErrorMessage.GreaterThan);
    }
}

