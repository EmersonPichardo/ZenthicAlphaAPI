using System.ComponentModel.DataAnnotations;

namespace Application.Behaviors;

public record PerformanceSettings
{
    [Required]
    public required int RequestProcessingThresholdMilliseconds { get; init; }
}