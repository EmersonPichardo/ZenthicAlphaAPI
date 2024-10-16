using Application.Auth;
using Application.Commands;

namespace Identity.Application.Users.RefreshToken;

[AllowAnonymous]
public record RefreshUserTokenCommand
    : ICommand<RefreshUserTokenCommandResponse>;
