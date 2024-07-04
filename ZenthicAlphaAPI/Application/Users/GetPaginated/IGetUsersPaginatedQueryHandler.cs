using Application._Common.Failures;
using Application._Common.Pagination;
using MediatR;
using OneOf;

namespace Application.Users.GetPaginated;

public interface IGetUsersPaginatedQueryHandler
    : IRequestHandler<GetUsersPaginatedQuery, OneOf<PaginatedList<GetUsersPaginatedQueryResponse>, Failure>>;