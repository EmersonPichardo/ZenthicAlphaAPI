using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record BackgroundTaskSettings
{
    [Required]
    public required TimeSpan TimeInterval { get; init; }
}
