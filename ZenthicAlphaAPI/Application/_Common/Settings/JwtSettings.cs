using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record JwtSettings
{
    [Required, MinLength(64)]
    public required string Secret { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required, Range(typeof(TimeSpan), "72:00:00", "4383:00:00")]
    public required TimeSpan RefreshTokenLifetime { get; init; }

    [Required, Range(typeof(TimeSpan), "00:01:00", "72:00:00")]
    public required TimeSpan TokenLifetime { get; init; }
}