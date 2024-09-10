using Application.Pagination;
using FluentValidation;
using Infrastructure.Validations.ValidationErrorMessages;

namespace Infrastructure.Validations.GenericValidators;

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

