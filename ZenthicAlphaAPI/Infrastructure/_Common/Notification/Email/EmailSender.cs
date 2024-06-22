using Application._Common.Notifications.Emails;
using Application._Common.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure._Common.Notification.Email;

internal class EmailSender : IEmailSender
{
    private readonly SmtpClient smtpClient;
    private readonly MailAddress sender;

    public EmailSender(IOptions<SmtpSettings> smtpSettingsOptions)
    {
        var smtpSettings = smtpSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(smtpSettingsOptions));

        smtpClient = new SmtpClient(smtpSettings.Server, smtpSettings.Port)
        {
            UseDefaultCredentials = !smtpSettings.HasCredentials,
            Credentials = smtpSettings.HasCredentials
                ? new NetworkCredential(smtpSettings.User, smtpSettings.Password)
                : null,
            EnableSsl = smtpSettings.EnableSsl
        };

        sender = new MailAddress(smtpSettings.FromEmail, smtpSettings.FromName);
    }

    public async Task SendAsync(
        string receiverEmail,
        string subject,
        string message,
        CancellationToken cancellationToken = default)
    {
        using var mailMessage = CreateMessage(receiverEmail, subject, message);
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }

    private MailMessage CreateMessage(string to, string subject, string message)
    {
        var receiver = new MailAddress(to);

        return new MailMessage(sender, receiver)
        {
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
    }
}
