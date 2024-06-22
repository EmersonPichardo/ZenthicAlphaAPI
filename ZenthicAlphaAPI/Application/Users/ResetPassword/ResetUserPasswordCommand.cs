using Application._Common.Security.Authorization;
using Domain._Common.Modularity;

namespace Application.Users.ResetPassword;

[Authorize(Component.Users, Permission.Update)]
public record ResetUserPasswordCommand
    : ICommand
{
    public required Guid Id { get; init; }
}
