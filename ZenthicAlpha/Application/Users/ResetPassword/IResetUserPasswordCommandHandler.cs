using MediatR;

namespace Application.Users.ResetPassword;

public interface IResetUserPasswordCommandHandler
    : IRequestHandler<ResetUserPasswordCommand>;
