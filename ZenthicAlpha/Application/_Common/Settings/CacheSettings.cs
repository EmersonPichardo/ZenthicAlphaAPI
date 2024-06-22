using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record CacheSettings
{
    [Required]
    public required string ConnectionString { get; init; }

    public TimeSpan? AbsoluteLifetime { get; init; }

    public TimeSpan? SlidingLifetime { get; init; }
}