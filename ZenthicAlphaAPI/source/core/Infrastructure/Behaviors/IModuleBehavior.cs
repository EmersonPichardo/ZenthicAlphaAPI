using Application.Failures;
using OneOf;
using OneOf.Types;

namespace Infrastructure.Behaviors;

public interface IModuleBehavior
{
    Task<OneOf<Success, Failure>> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : notnull;
}
