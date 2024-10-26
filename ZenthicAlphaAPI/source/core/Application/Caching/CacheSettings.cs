using System.ComponentModel.DataAnnotations;

namespace Application.Caching;

public record CacheSettings
{
    [Required]
    public required string ConnectionString { get; init; }

    public TimeSpan? AbsoluteLifetime { get; init; }

    public TimeSpan? SlidingLifetime { get; init; }
}