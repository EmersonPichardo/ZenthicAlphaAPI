using MediatR;

namespace Application.Users.ClearSession;

public interface IClearUserSessionCommandHandler
    : IRequestHandler<ClearUserSessionCommand>;
