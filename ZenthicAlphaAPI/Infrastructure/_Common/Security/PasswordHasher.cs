using Application._Common.Security.Authentication;
using Application._Common.Settings;
using Infrastructure._Persistence.Databases.ApplicationDbContext.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Infrastructure._Common.Security;

internal class PasswordHasher(
    IOptions<HashingSettings> hashingSettingsOptions
)
    : IPasswordHasher
{
    private readonly HashingSettings hashingSettings = hashingSettingsOptions.Value;
    private readonly short algorithmByteLength = (short)(UserConfiguration.PasswordLength / 2);
    private readonly short saltBytesLength = (short)(UserConfiguration.SaltLength / 2);

    public (string hashedPassword, string salt, string algorithm, short iterations) Generate(string password)
    {
        var algorithm = new HashAlgorithmName(hashingSettings.Algorithm);
        var hashOperator = new Rfc2898DeriveBytes(password, saltBytesLength, hashingSettings.Iterations, algorithm);
        var hashedPasswordBytes = hashOperator.GetBytes(algorithmByteLength);

        return (
            ConvertToHexString(hashedPasswordBytes),
            ConvertToHexString(hashOperator.Salt),
            hashingSettings.Algorithm,
            hashingSettings.Iterations
        );
    }

    public (string newPassword, string hashedPassword, string salt, string algorithm, short iterations) GenerateNewPassword()
    {
        var newPassword = NewStringPassword();
        (var hashedPassword, var salt, var algorithm, var iterations) = Generate(newPassword);

        return (newPassword, hashedPassword, salt, algorithm, iterations);
    }

    public string Hash(string password, string salt, HashingSettings hashingSettings)
    {
        var algorithm = new HashAlgorithmName(hashingSettings.Algorithm);
        var saltBytes = Convert.FromHexString(salt);
        var hashOperator = new Rfc2898DeriveBytes(password, saltBytes, hashingSettings.Iterations, algorithm);
        var hashedPasswordBytes = hashOperator.GetBytes(algorithmByteLength);

        return ConvertToHexString(hashedPasswordBytes);
    }

    private static string ConvertToHexString(byte[] data)
        => BitConverter.ToString(data).Replace("-", "").ToLower();
    private static string NewStringPassword()
    {
        const string validChars = "AaBbCcDdEeFfGgHhJjKkMmNnPpQqRrSsTtUuVvWwXxYyZz123456789!@#$%&*_+=.?-";
        const int passwordLength = 16;

        using var random = RandomNumberGenerator.Create();
        var randomBytes = new byte[passwordLength];
        random.GetBytes(randomBytes);

        var passwordChars = new char[passwordLength];

        for (var index = 0; index < passwordLength; index++)
        {
            var randomIndex = randomBytes[index] % validChars.Length;
            passwordChars[index] = validChars[randomIndex];
        }

        return new(passwordChars);
    }
}