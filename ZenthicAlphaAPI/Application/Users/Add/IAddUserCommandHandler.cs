using MediatR;

namespace Application.Users.Add;

public interface IAddUserCommandHandler
    : IRequestHandler<AddUserCommand>;
