namespace Application.Notifications.Emails;

public interface IEmailSender
{
    Task SendAsync(string receiverEmail, string subject, string message, CancellationToken cancellationToken = default);
}