using Domain.Security;

namespace Application.Users.Login;

public record UserLoggedInEvent
    : BaseEntityEvent<User>
{
    public required LoginUserCommandResponse Session { get; init; }
}
