using Application.Notifications.Emails;
using Identity.Application.Users.ResetPassword;
using MediatR;

namespace Identity.Infrastructure.Users.ResetPassword;

internal class UserPasswordResetEventSendPasswordHandler(
    IEmailSender emailSender
)
    : INotificationHandler<UserPasswordResetEvent>
{
    public async Task Handle(UserPasswordResetEvent @event, CancellationToken cancellationToken)
    {
        var message = EmailTemplateCollection
            .ResetPasswordTemplate
            .Replace("[ReceiverFullName]", @event.User.UserName)
            .Replace("[NewPassword]", @event.NewPassword);

        await emailSender.SendAsync(
            @event.User.Email,
            $"Contraseña restablecida",
            message,
            cancellationToken
        );
    }
}
