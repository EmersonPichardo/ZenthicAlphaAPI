using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Users.Logout;

public interface ILogoutCurrentUserCommandHandler
    : IRequestHandler<LogoutCurrentUserCommand, OneOf<None, Failure>>;
