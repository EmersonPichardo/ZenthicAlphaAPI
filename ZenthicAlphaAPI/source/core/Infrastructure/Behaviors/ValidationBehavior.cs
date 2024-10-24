using Application.Auth;
using Application.Failures;
using FluentValidation;
using MediatR;

namespace Infrastructure.Behaviors;

internal class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    IUserSessionService userSessionInfo
)
    : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorsCount = validators.TryGetNonEnumeratedCount(out var count)
            ? count : validators.Count();

        if (validatorsCount is 0)
            return await next().ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);

        context.RootContextData[nameof(IUserSession)] = await userSessionInfo.GetSessionAsync();

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

        return await next();
    }
}
