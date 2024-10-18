using System.Security.Cryptography;

namespace Identity.Infrastructure.Common.Auth;

internal class PasswordManager(
    HashingManager hashingManager
)
{
    public NewPasswordResult Generate()
    {
        var newPassword = NewStringPassword();
        var passwordResult = Generate(newPassword);

        return new()
        {
            NewPassword = newPassword,
            HashedPassword = passwordResult.HashedPassword,
            HashingStamp = passwordResult.HashingStamp
        };
    }
    public PasswordResult Generate(string password)
    {
        var hashingResult = hashingManager.Hash(password);

        return new()
        {
            HashedPassword = hashingResult.HashedValue,
            HashingStamp = hashingResult.HashingStamp
        };
    }
    public bool Equals(string password, string hashedPassword, string hashingStamp)
    {
        return hashingManager.Equals(password, hashedPassword, hashingStamp);
    }

    private static string NewStringPassword()
    {
        const string validChars = "AaBbCcDdEeFfGgHhJjKkMmNnPpQqRrSsTtUuVvWwXxYyZz123456789!@#$/*_+=.?-";
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

    public record PasswordResult
    {
        public required string HashedPassword { get; init; }
        public required string HashingStamp { get; init; }
    }
    public record NewPasswordResult : PasswordResult
    {
        public required string NewPassword { get; init; }
    }
}