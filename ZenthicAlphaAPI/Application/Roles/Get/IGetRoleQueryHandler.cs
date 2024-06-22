using MediatR;

namespace Application.Roles.Get;

public interface IGetRoleQueryHandler
    : IRequestHandler<GetRoleQuery, GetRoleQueryResponse>;
