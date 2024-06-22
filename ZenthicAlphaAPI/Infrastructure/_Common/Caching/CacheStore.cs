using Application._Common.Caching;
using Application._Common.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure._Common.Caching;

internal class CacheStore : ICacheStore
{
    private readonly IDistributedCache distributedCache;
    private readonly ConcurrentDictionary<string, string> cachedCompoundKeys;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly DistributedCacheEntryOptions defaultCacheOptions;

    public CacheStore(IDistributedCache distributedCache, IOptions<CacheSettings> cacheSettingsOptions)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        cachedCompoundKeys = [];

        jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        var cacheSettings = cacheSettingsOptions.Value;
        defaultCacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = cacheSettings.AbsoluteLifetime,
            SlidingExpiration = cacheSettings.SlidingLifetime
        };
    }

    public async Task SetAsync<T>(string tag, string key, T value, CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(
            value,
            jsonSerializerOptions
        );

        await distributedCache.SetStringAsync(
            key,
            serializedValue,
            defaultCacheOptions,
            cancellationToken
        );

        cachedCompoundKeys.TryAdd(key, tag);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var deserializedValue = await distributedCache.GetStringAsync(
            key,
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(deserializedValue))
            return default;

        return JsonSerializer.Deserialize<T>(
            deserializedValue,
            jsonSerializerOptions
        );
    }

    public async Task ClearAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
        cachedCompoundKeys.TryRemove(key, out _);
    }

    public async Task ClearByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        var clearCacheTasks = cachedCompoundKeys
            .Where(pair => pair.Value.Equals(tag, StringComparison.OrdinalIgnoreCase))
            .Select(pair => ClearAsync(pair.Key, cancellationToken));

        await Task.WhenAll(clearCacheTasks);
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        var clearCacheTasks = cachedCompoundKeys
            .Select(pair => ClearAsync(pair.Key, cancellationToken));

        await Task.WhenAll(clearCacheTasks);
    }
}