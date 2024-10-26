using Application.Auth;
using Application.Commands;

namespace Identity.Application.Auth.UpdateOAuthUser;

[AuthorizeOAuth]
public record UpdateOAuthUserCommand
    : ICommand;

