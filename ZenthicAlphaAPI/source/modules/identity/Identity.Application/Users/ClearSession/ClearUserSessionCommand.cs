using Application.Authorization;
using Application.Commands;

namespace Identity.Application.Users.ClearSession;

[AllowAnonymous]
public record ClearUserSessionCommand
    : ICommand
{
    public required Guid UserId { get; init; }
}
