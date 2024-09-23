using System.ComponentModel.DataAnnotations;

namespace Application.Settings;

public record BackgroundTaskSettings
{
    [Required]
    public required TimeSpan TimeInterval { get; init; }
}
