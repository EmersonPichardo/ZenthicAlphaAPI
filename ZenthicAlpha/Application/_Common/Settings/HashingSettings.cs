using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record HashingSettings
{
    [Required]
    public required string Algorithm { get; init; }

    [Required, Range(1000, 1000000)]
    public required short Iterations { get; init; }
}