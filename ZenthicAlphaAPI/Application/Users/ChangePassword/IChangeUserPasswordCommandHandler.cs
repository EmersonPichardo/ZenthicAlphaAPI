using MediatR;

namespace Application.Users.ChangePassword;

public interface IChangeUserPasswordCommandHandler
    : IRequestHandler<ChangeUserPasswordCommand>;