using Application.Authorization;
using Application.Commands;
using Domain.Modularity;

namespace Identity.Application.Users.ResetPassword;

[Authorize(Component.Users, Permission.Update)]
public record ResetUserPasswordCommand
    : ICommand
{
    public required Guid Id { get; init; }
}
