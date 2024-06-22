namespace Application._Common.Caching;

public interface ICacheStore
{
    Task SetAsync<T>(string tag, string key, T value, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task ClearAsync(string key, CancellationToken cancellationToken = default);
    Task ClearByTagAsync(string tag, CancellationToken cancellationToken = default);
    Task ClearAllAsync(CancellationToken cancellationToken = default);
}