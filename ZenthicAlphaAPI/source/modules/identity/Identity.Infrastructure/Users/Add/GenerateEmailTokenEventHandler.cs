using Identity.Application.Users.Add;
using Identity.Application.Users.GenerateEmailToken;
using MediatR;

namespace Identity.Infrastructure.Users.Add;

internal class GenerateEmailTokenEventHandler(
    ISender mediator
)
    : INotificationHandler<UserAddedEvent>
{
    public async Task Handle(UserAddedEvent @event, CancellationToken cancellationToken)
    {
        var command = new GenerateEmailTokenCommand { UserId = @event.User.Id };
        await mediator.Send(command, cancellationToken);
    }
}
