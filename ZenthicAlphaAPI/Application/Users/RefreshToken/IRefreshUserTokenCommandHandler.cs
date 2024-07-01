using MediatR;

namespace Application.Users.RefreshToken;

public interface IRefreshUserTokenCommandHandler
    : IRequestHandler<RefreshUserTokenCommand, RefreshUserTokenCommandResponse>;
