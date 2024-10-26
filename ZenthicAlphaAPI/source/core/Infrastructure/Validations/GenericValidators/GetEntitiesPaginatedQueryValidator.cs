using Application.Pagination;
using Application.Validations;
using FluentValidation;

namespace Infrastructure.Validations.GenericValidators;

internal abstract class GetEntitiesPaginatedQueryValidator<TQuery>
    : AbstractValidator<TQuery>
    where TQuery : IGetEntitiesPaginatedQuery
{
    protected GetEntitiesPaginatedQueryValidator()
    {
        RuleFor(query => query.CurrentPage)
            .GreaterThan(0)
                .WithMessage(GenericValidationErrorMessage.GreaterThan);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
                .WithMessage(GenericValidationErrorMessage.GreaterThan);
    }
}

