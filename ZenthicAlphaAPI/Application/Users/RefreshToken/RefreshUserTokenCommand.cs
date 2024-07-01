using Application._Common.Security.Authorization;

namespace Application.Users.RefreshToken;

[AllowAnonymous]
public record RefreshUserTokenCommand
    : ICommand<RefreshUserTokenCommandResponse>;
