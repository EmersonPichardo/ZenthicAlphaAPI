using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using AutoMapper;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.GetPaginated;
using Identity.Domain.User;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Users.GetPaginated;

internal class GetUsersPaginatedQueryHandler(IIdentityDbContext dbContext, IMapper mapper)
    : GetEntitiesPaginatedQueryHandler<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse, User>(dbContext, mapper, GetFilterExpression)
    , IRequestHandler<GetUsersPaginatedQuery, OneOf<PaginatedList<GetUsersPaginatedQueryResponse>, Failure>>
{
    private static Expression<Func<GetUsersPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return user
            => string.IsNullOrWhiteSpace(search)
            || user.FullName.ToNormalize().Contains(search)
            || user.Email.ToNormalize().Contains(search);
    }
}