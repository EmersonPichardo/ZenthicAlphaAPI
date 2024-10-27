using Application.Commands;
using Identity.Application.Auth;

namespace Identity.Application.Users.RefreshToken;

[AuthorizeRefreshToken]
public record RefreshUserTokenCommand
    : ICommand<RefreshUserTokenCommandResponse>;
