using Identity.Application.Common.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Identity.Infrastructure.Common.Auth;

internal class HashingManager(
    IOptions<AuthSettings> authSettingsOptions
)
{
    private readonly AuthSettings.HashingSettings hashingSettings = authSettingsOptions.Value.Hashing;

    public HashingResult Hash(string value)
    {
        var algorithm = new HashAlgorithmName(hashingSettings.AlgorithmName);
        var hashOperator = new Rfc2898DeriveBytes(value, hashingSettings.SaltBytesLength, hashingSettings.Iterations, algorithm);
        var hashedPasswordBytes = hashOperator.GetBytes(hashingSettings.AlgorithmBytesLength);
        var salt = Convert.ToBase64String(hashOperator.Salt);

        return new()
        {
            HashedValue = Convert.ToBase64String(hashedPasswordBytes),
            HashingStamp = HashingStamp.Encode(hashingSettings, salt)
        };
    }
    public HashingResult Hash(string value, string hashingStamp)
    {
        var hashingStampValue = HashingStamp.Decode(hashingStamp)
            ?? throw new ArgumentException("Unable to decode user hashing stamp", nameof(hashingStamp));

        var algorithm = new HashAlgorithmName(hashingStampValue.AlgorithmName);
        var saltBytes = Convert.FromBase64String(hashingStampValue.Salt);
        var hashOperator = new Rfc2898DeriveBytes(value, saltBytes, hashingStampValue.Iterations, algorithm);
        var hashedValueBytes = hashOperator.GetBytes(hashingStampValue.AlgorithmBytesLength);

        return new()
        {
            HashedValue = Convert.ToBase64String(hashedValueBytes),
            HashingStamp = hashingStamp
        };
    }
    public bool Equals(string value, string hashedValue, string hashingStamp)
    {
        var HashedPlainValue = Hash(value, hashingStamp).HashedValue;

        var HashedPlainValueBytes = Convert.FromBase64String(HashedPlainValue);
        var hashedValueBytes = Convert.FromBase64String(hashedValue);

        return CryptographicOperations.FixedTimeEquals(
            HashedPlainValueBytes.AsSpan(), hashedValueBytes.AsSpan()
        );
    }

    public record HashingResult
    {
        public required string HashedValue { get; init; }
        public required string HashingStamp { get; init; }
    }
    private sealed record HashingStamp
    {
        public required string AlgorithmName { get; init; }
        public required int AlgorithmBytesLength { get; init; }
        public required int Iterations { get; init; }
        public required string Salt { get; init; }

        [JsonConstructor] private HashingStamp() { }

        public static string Encode(AuthSettings.HashingSettings hashingSettings, string salt)
        {
            var hashingStamp = new HashingStamp
            {
                AlgorithmName = hashingSettings.AlgorithmName,
                AlgorithmBytesLength = hashingSettings.AlgorithmBytesLength,
                Iterations = hashingSettings.Iterations,
                Salt = salt
            };
            var jsonString = JsonSerializer.Serialize(hashingStamp);
            var bytes = Encoding.Default.GetBytes(jsonString);

            return Convert.ToBase64String(bytes);
        }
        public static HashingStamp? Decode(string hashingStamp)
        {
            var hashingStampBytes = Convert.FromBase64String(hashingStamp);
            var hashingStampJson = Encoding.Default.GetString(hashingStampBytes);

            return JsonSerializer.Deserialize<HashingStamp>(hashingStampJson);
        }
    }
}