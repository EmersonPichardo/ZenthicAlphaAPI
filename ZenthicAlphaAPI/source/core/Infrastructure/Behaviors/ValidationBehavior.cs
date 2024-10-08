﻿using Application.Failures;
using FluentValidation;
using MediatR;

namespace Infrastructure.Behaviors;

internal class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
)
    : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var isValidatorsCountGot = validators.TryGetNonEnumeratedCount(out var validatorsCount);

        if (isValidatorsCountGot ? validatorsCount is 0 : !validators.Any())
            return await next().ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);

        var validationTasks = validators
            .Select(validator =>
                validator.ValidateAsync(context, cancellationToken)
            );

        var validationResults = await Task.WhenAll(validationTasks);

        var failures = validationResults
            .Where(result => result.Errors.Count != 0)
            .SelectMany(result => result.Errors);

        var isFailuresCountGot = failures.TryGetNonEnumeratedCount(out var failuresCount);

        if (isFailuresCountGot ? failuresCount > 0 : failures.Any())
            return (dynamic)FailureFactory.InvalidRequest("Invalid request", failures);

        return await next().ConfigureAwait(false);
    }
}
