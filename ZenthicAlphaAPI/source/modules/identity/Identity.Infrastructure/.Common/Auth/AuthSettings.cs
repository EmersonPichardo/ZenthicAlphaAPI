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

    [Required]
    public OAuthSettings? OAuth { get; init; }

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
    internal record OAuthSettings
    {
        public ProviderSettings? Google { get; init; }

        internal record ProviderSettings
        {
            [Required]
            public required string ClientId { get; init; }

            [Required]
            public required string ClientSecret { get; init; }
        }
    }
}