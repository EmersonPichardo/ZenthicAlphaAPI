using Application._Common.Caching;
using Application._Common.Commands;
using Application._Common.Queries;
using MediatR;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure._Common.Behaviors;

internal class CachingBehavior<TRequest, TResponse>(
    ICacheStore cacheStore
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : new()
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheAttribute = request
            .GetType()
            .GetCustomAttribute<CacheAttribute>();

        if (cacheAttribute is null)
            return await next();

        var tag = cacheAttribute.GetTag();

        var response = request switch
        {
            IBaseQuery => await QueryCacheHandlerAsync(tag, request, next, cancellationToken),
            IBaseCommand => await CommandCacheHandlerAsync(tag, next, cancellationToken),
            _ => await next()
        };

        return response;
    }

    private async Task<TResponse> QueryCacheHandlerAsync(
        string tag,
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var key = $"{request.GetType().Name} {JsonSerializer.Serialize(request)}";
        var cachedResponse = await cacheStore.GetAsync<TResponse>(key, cancellationToken);

        if (cachedResponse is not null)
            return cachedResponse;

        var response = await next();

        await cacheStore.SetAsync(tag, key, await next(), cancellationToken);

        return response;
    }
    private async Task<TResponse> CommandCacheHandlerAsync(
        string tag,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await cacheStore.ClearByTagAsync(tag, cancellationToken);

        return await next();
    }
}