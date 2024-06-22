using Application._Common.Pagination;
using MediatR;

namespace Application.Roles.GetPaginated;

public interface IGetRolesPaginatedQueryHandler
    : IRequestHandler<GetRolesPaginatedQuery, PaginatedList<GetRolesPaginatedQueryResponse>>;