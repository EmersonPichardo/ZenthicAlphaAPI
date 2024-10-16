using System.ComponentModel.DataAnnotations;

namespace Identity.Infrastructure.Common.Settings;

internal record AuthSettings
{
    [Required]
    public required JwtSettings Jwt { get; init; }

    [Required]
    public required HashingSettings Hashing { get; init; }

    [Required]
    public required TokenSettings Token { get; init; }

    internal record JwtSettings
    {
        [Required, MinLength(64)]
        public required string Key { get; init; }

        [Required]
        public required TimeSpan RefreshTokenLifetime { get; init; }

        [Required]
        public required TimeSpan TokenLifetime { get; init; }
    }
    internal record HashingSettings
    {
        [Required]
        public required string AlgorithmName { get; init; }

        [Required, MinLength(50000)]
        public required int Iterations { get; init; }

        [Required]
        public required int AlgorithmBytesLength { get; init; }

        [Required]
        public required int SaltBytesLength { get; init; }
    }
    internal record TokenSettings
    {
        [Required]
        public required int Length { get; init; }

        [Required]
        public required string ValidChars { get; init; }

        [Required]
        public required TimeSpan LifeTime { get; init; }
    }
}