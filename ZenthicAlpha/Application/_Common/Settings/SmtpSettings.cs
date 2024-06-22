using System.ComponentModel.DataAnnotations;

namespace Application._Common.Settings;

public record SmtpSettings
{
    [Required]
    public required string Server { get; init; }

    [Required]
    public required int Port { get; init; }

    [Required, EmailAddress]
    public required string FromEmail { get; init; }

    [Required]
    public required string FromName { get; init; }

    public bool EnableSsl { get; init; } = false;

    public string? User { get; init; }
    public string? Password { get; init; }
    public bool HasCredentials => !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Password);
}
