using Identity.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Common.Auth;

internal class TokenManager(
    IOptions<AuthSettings> authSettingsOptions,
    HashingManager hashingManager
)
{
    private readonly AuthSettings.TokenSettings tokenSettings = authSettingsOptions.Value.Token;

    public TokenResult Generate()
    {
        var randomChars = Random.Shared.GetItems(
            tokenSettings.ValidChars.AsSpan(),
            tokenSettings.Length
        );
        var token = new string(randomChars);
        var hashedResult = hashingManager.Hash(token);

        return new()
        {
            Token = token,
            HashedToken = hashedResult.HashedValue,
            HashingStamp = hashedResult.HashingStamp
        };
    }

    public record TokenResult
    {
        public required string Token { get; init; }
        public required string HashedToken { get; init; }
        public required string HashingStamp { get; init; }
    }
}