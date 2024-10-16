using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using Identity.Application.Users.GetPaginated;
using Identity.Domain.User;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Users.GetPaginated;

internal class GetUsersPaginatedQueryHandler(IdentityModuleDbContext dbContext)
    : GetEntitiesPaginatedQueryHandler<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse, User>(dbContext)
    , IRequestHandler<GetUsersPaginatedQuery, OneOf<PaginatedList<GetUsersPaginatedQueryResponse>, Failure>>
{
    protected override Expression<Func<GetUsersPaginatedQueryResponse, bool>> GetFilterExpression(string? filter)
    {
        return user
            => string.IsNullOrWhiteSpace(filter)
            || user.UserName.ToNormalize().Contains(filter)
            || user.Email.ToNormalize().Contains(filter);
    }
    protected override Expression<Func<User, GetUsersPaginatedQueryResponse>> MapToResponse()
    {
        return user => new GetUsersPaginatedQueryResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Status = user.Status.ToString()
        };
    }
}