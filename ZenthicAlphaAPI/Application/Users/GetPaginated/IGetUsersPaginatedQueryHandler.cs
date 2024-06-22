using Application._Common.Pagination;
using MediatR;

namespace Application.Users.GetPaginated;

public interface IGetUsersPaginatedQueryHandler
    : IRequestHandler<GetUsersPaginatedQuery, PaginatedList<GetUsersPaginatedQueryResponse>>;