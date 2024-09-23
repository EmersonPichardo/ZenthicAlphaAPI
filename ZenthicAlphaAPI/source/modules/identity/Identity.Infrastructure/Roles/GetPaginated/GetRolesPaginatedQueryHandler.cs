using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.GetPaginated;
using Identity.Domain.Roles;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Roles.GetPaginated;

internal class GetRolesPaginatedQueryHandler(IIdentityDbContext dbContext)
    : GetEntitiesPaginatedQueryHandler<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse, Role>(dbContext, GetFilterExpression)
    , IRequestHandler<GetRolesPaginatedQuery, OneOf<PaginatedList<GetRolesPaginatedQueryResponse>, Failure>>
{
    protected override Expression<Func<Role, GetRolesPaginatedQueryResponse>> MapToResponse()
    {
        return role => new GetRolesPaginatedQueryResponse()
        {
            Id = role.Id,
            Name = role.Name
        };
    }

    private static Expression<Func<GetRolesPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return role
            => string.IsNullOrWhiteSpace(search)
            || role.Name.ToNormalize().Contains(search);
    }
}