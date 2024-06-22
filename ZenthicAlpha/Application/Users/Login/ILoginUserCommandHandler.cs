using MediatR;

namespace Application.Users.Login;

public interface ILoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginUserCommandResponse>;
