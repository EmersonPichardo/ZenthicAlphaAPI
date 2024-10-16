using Application.Notifications.Emails;
using Identity.Application.Users.Add;
using Identity.Domain.User;
using Identity.Infrastructure.Common.Auth;
using Identity.Infrastructure.Common.Settings;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Users.Add;

internal class UserAddedEventSendPasswordHandler(
    IOptions<AuthSettings> authSettingsOptions,
    TokenManager tokenManager,
    IdentityModuleDbContext dbContext,
    IEmailSender emailSender
)
    : INotificationHandler<UserAddedEvent>
{
    private readonly AuthSettings.TokenSettings tokenSettings = authSettingsOptions.Value.Token;
    private readonly TokenManager.TokenResult tokenResult = tokenManager.Generate();

    public async Task Handle(UserAddedEvent @event, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Task> tasks = [
            SaveTokenAsync(@event, cancellationToken),
            SendWelcomeEmailAsync(@event, cancellationToken)
        ];

        await Task.WhenAll(tasks);
    }

    private async Task SaveTokenAsync(UserAddedEvent @event, CancellationToken cancellationToken)
    {
        var userToken = new UserToken
        {
            UserId = @event.User.Id,
            Token = tokenResult.HashedToken,
            HashingStamp = tokenResult.HashingStamp,
            Expiration = DateTime.UtcNow.Add(tokenSettings.LifeTime)
        };

        dbContext.UserTokens.Add(userToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    private async Task SendWelcomeEmailAsync(UserAddedEvent @event, CancellationToken cancellationToken)
    {
        var message = EmailTemplateCollection
            .AddUserTemplate
            .Replace("[ReceiverFullName]", @event.User.UserName)
            .Replace("[Token]", tokenResult.Token);

        await emailSender.SendAsync(
            @event.User.Email,
            $"¡Bienvenido/a a la aplicación {@event.User.UserName}!",
            message,
            cancellationToken
        );
    }
}
