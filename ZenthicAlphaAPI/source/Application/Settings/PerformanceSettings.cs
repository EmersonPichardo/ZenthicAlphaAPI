using System.ComponentModel.DataAnnotations;

namespace Application.Settings;

public record PerformanceSettings
{
    [Required]
    public required int RequestProcessingThresholdMilliseconds { get; init; }
}