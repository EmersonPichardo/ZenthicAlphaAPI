using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Users.Login;

public interface ILoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, OneOf<LoginUserCommandResponse, Failure>>;
