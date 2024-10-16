using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using Identity.Application.Roles.GetPaginated;
using Identity.Domain.Roles;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Roles.GetPaginated;

internal class GetRolesPaginatedQueryHandler(IdentityModuleDbContext dbContext)
    : GetEntitiesPaginatedQueryHandler<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse, Role>(dbContext)
    , IRequestHandler<GetRolesPaginatedQuery, OneOf<PaginatedList<GetRolesPaginatedQueryResponse>, Failure>>
{
    protected override Expression<Func<GetRolesPaginatedQueryResponse, bool>> GetFilterExpression(string? filter)
    {
        return role
            => string.IsNullOrWhiteSpace(filter)
            || role.Name.ToNormalize().Contains(filter);
    }
    protected override Expression<Func<Role, GetRolesPaginatedQueryResponse>> MapToResponse()
    {
        return role => new GetRolesPaginatedQueryResponse
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}