﻿using Application.Queries;
using Application.Validations;
using FluentValidation;

namespace Infrastructure.Validations.GenericValidators;

internal abstract class GetEntityQueryValidator<TQuery>
    : AbstractValidator<TQuery>
    where TQuery : IGetEntityQuery
{
    protected GetEntityQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty()
                .WithMessage(GenericValidationErrorMessage.Required);
    }
}
