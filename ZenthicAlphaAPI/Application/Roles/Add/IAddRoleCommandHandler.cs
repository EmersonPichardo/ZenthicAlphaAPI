using MediatR;

namespace Application.Roles.Add;

public interface IAddRoleCommandHandler
    : IRequestHandler<AddRoleCommand>;
