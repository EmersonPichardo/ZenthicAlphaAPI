using MediatR;

namespace Application.Roles.Update;

public interface IUpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand>;
