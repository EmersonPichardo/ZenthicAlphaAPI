using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record JwtSettings
{
    [Required, MinLength(64)]
    public required string Secret { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required TimeSpan RefreshTokenLifetime { get; init; }

    [Required]
    public required TimeSpan TokenLifetime { get; init; }
}