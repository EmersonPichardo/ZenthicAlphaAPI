using Application.Events;
using Identity.Domain.User;

namespace Identity.Application.Users.Login;

public record UserLoggedInEvent
    : BaseEntityEvent<User>
{
    public required LoginUserCommandResponse Session { get; init; }
}
