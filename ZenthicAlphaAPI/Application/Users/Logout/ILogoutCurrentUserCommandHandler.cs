using MediatR;

namespace Application.Users.Logout;

public interface ILogoutCurrentUserCommandHandler
    : IRequestHandler<LogoutCurrentUserCommand>;
