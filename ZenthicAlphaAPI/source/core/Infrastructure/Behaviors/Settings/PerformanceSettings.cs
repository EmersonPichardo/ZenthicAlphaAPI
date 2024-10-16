using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Behaviors.Settings;

internal record PerformanceSettings
{
    [Required]
    public required int RequestProcessingThresholdMilliseconds { get; init; }
}