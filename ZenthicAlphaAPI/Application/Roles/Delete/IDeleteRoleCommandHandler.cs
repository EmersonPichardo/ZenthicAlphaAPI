using MediatR;

namespace Application.Roles.Delete;

public interface IDeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand>;
