using FluentValidation;
using MediatR;
using ValidationException = Application._Common.Exceptions.ValidationException;

namespace Infrastructure._Common.Behaviors;

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
            return await next();

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
            throw new ValidationException(failures);

        return await next();
    }
}
