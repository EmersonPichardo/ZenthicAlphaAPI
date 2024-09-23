using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.GetPaginated;
using Identity.Domain.User;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Users.GetPaginated;

internal class GetUsersPaginatedQueryHandler(IIdentityDbContext dbContext)
    : GetEntitiesPaginatedQueryHandler<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse, User>(dbContext, GetFilterExpression)
    , IRequestHandler<GetUsersPaginatedQuery, OneOf<PaginatedList<GetUsersPaginatedQueryResponse>, Failure>>
{
    protected override Expression<Func<User, GetUsersPaginatedQueryResponse>> MapToResponse()
    {
        return user => new GetUsersPaginatedQueryResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Status = user.Status.ToString()
        };
    }

    private static Expression<Func<GetUsersPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return user
            => string.IsNullOrWhiteSpace(search)
            || user.FullName.ToNormalize().Contains(search)
            || user.Email.ToNormalize().Contains(search);
    }
}