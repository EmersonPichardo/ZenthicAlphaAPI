using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Common.Settings;

public record AuthSettings
{
    [Required]
    public required JwtSettings Jwt { get; init; }

    [Required]
    public required HashingSettings Hashing { get; init; }

    [Required]
    public required TokenSettings Token { get; init; }

    public OAuthSettings? OAuth { get; init; }

    public record JwtSettings
    {
        [Required, MinLength(64)]
        public required string Key { get; init; }

        [Required]
        public required TimeSpan RefreshTokenLifetime { get; init; }

        [Required]
        public required TimeSpan TokenLifetime { get; init; }
    }
    public record HashingSettings
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
    public record TokenSettings
    {
        [Required]
        public required int Length { get; init; }

        [Required]
        public required string ValidChars { get; init; }

        [Required]
        public required TimeSpan LifeTime { get; init; }
    }
    public record OAuthSettings
    {
        public ProviderSettings? Google { get; init; }
        public ProviderSettings? Microsoft { get; init; }
        public ProviderSettings? Facebook { get; init; }

        public record ProviderSettings
        {
            [Required]
            public required string ClientId { get; init; }

            [Required]
            public required string ClientSecret { get; init; }
        }
    }
}