using Application.Notifications.Emails;
using Identity.Application.Users.GenerateEmailToken;
using MediatR;

namespace Identity.Infrastructure.Users.GenerateEmailToken;

internal class EmailTokenGeneratedEventHandler(
    IEmailSender emailSender
)
    : INotificationHandler<EmailTokenGeneratedEvent>
{
    public async Task Handle(EmailTokenGeneratedEvent @event, CancellationToken cancellationToken)
    {
        var message = EmailTemplateCollection
            .ConfirmationTokenTemplate
            .Replace("[ReceiverFullName]", @event.User.UserName)
            .Replace("[Token]", @event.Token);

        await emailSender.SendAsync(
            @event.User.Email,
            $"¡Hola {@event.User.UserName}!",
            message,
            cancellationToken
        );
    }
}
