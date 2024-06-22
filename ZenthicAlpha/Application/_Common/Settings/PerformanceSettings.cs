using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record PerformanceSettings
{
    [Required]
    public required int RequestProcessingThresholdMilliseconds { get; init; }
}