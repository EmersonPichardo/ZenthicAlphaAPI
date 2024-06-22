using Application._Common.Notifications.Emails;
using Application.Users.Add;
using MediatR;

namespace Infrastructure.Users.Add;

internal class UserAddedEventSendPasswordHandler(
    IEmailSender emailSender
)
    : INotificationHandler<UserAddedEvent>
{
    public async Task Handle(UserAddedEvent @event, CancellationToken cancellationToken)
    {
        var message = EmailTemplateCollection
            .AddUserTemplate
            .Replace("[ReceiverFullName]", @event.FullName)
            .Replace("[NewPassword]", @event.NewPassword);

        await emailSender.SendAsync(
            @event.Email,
            $"¡Bienvenido/a a la aplicación {@event.FullName}!",
            message,
            cancellationToken
        );
    }
}
