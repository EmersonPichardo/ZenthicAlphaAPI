using Application.Caching;
using Application.Commands;
using Application.Helpers;
using Application.Queries;
using MediatR;
using OneOf;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Behaviors;

internal class CachingBehavior<TRequest, TResponse>(
    ICacheStore cacheStore
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IOneOf
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
            return await next().ConfigureAwait(false);

        var tag = cacheAttribute.GetTag();

        var response = request switch
        {
            IBaseQuery => await QueryCacheHandlerAsync(tag, request, next, cancellationToken),
            IBaseCommand => await CommandCacheHandlerAsync(tag, next, cancellationToken),
            _ => await next().ConfigureAwait(false)
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
        var responseType = typeof(TResponse).GenericTypeArguments[0];
        var cachedResponse = await cacheStore.GetAsync(key, responseType, cancellationToken);

        if (cachedResponse is not null)
            return (dynamic)cachedResponse;

        var response = await next().ConfigureAwait(false);

        if (response.IsSuccess())
            await cacheStore.SetAsync(tag, key, response.Value, cancellationToken);

        return response;
    }
    private async Task<TResponse> CommandCacheHandlerAsync(
        string tag,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await cacheStore.ClearByTagAsync(tag, cancellationToken);

        return await next().ConfigureAwait(false);
    }
}