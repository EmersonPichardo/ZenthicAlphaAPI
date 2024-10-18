using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Behaviors;

internal class AuthorizationBehavior<TRequest, TResponse>(
    [FromKeyedServices(BehaviorsConstants.AuthorizationBehaviorName)] IModuleBehavior authorizationModuleBehavior
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await next();

        var authorizationModuleBehaviorResult = await authorizationModuleBehavior
            .HandleAsync<TRequest, TResponse>(request, cancellationToken);

        return await authorizationModuleBehaviorResult.Match<Task<TResponse>>(
            async success => await next(),
            async failure => await Task.FromResult((dynamic)failure)
        );
    }
}
