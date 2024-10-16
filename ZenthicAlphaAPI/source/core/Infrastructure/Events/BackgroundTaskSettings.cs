using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Events;

internal record BackgroundTaskSettings
{
    [Required]
    public required TimeSpan TimeInterval { get; init; }
}
