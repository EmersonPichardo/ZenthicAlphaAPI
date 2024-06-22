using MediatR;

namespace Application.Roles.GetAll;

public interface IGetAllRolesQueryHandler
    : IRequestHandler<GetAllRolesQuery, IList<GetAllRolesQueryResponse>>;
