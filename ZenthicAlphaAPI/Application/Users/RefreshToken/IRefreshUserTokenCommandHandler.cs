using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Users.RefreshToken;

public interface IRefreshUserTokenCommandHandler
    : IRequestHandler<RefreshUserTokenCommand, OneOf<RefreshUserTokenCommandResponse, Failure>>;
