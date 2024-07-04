using Application._Common.Failures;
using Application._Common.Pagination;
using MediatR;
using OneOf;

namespace Application.Roles.GetPaginated;

public interface IGetRolesPaginatedQueryHandler
    : IRequestHandler<GetRolesPaginatedQuery, OneOf<PaginatedList<GetRolesPaginatedQueryResponse>, Failure>>;