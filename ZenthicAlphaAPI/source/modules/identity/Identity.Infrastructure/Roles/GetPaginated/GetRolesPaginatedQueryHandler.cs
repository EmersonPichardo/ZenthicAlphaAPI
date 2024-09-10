using Application.Failures;
using Application.Helpers;
using Application.Pagination;
using AutoMapper;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.GetPaginated;
using Identity.Domain.Roles;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Roles.GetPaginated;

internal class GetRolesPaginatedQueryHandler(IIdentityDbContext dbContext, IMapper mapper)
    : GetEntitiesPaginatedQueryHandler<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse, Role>(dbContext, mapper, GetFilterExpression)
    , IRequestHandler<GetRolesPaginatedQuery, OneOf<PaginatedList<GetRolesPaginatedQueryResponse>, Failure>>
{
    private static Expression<Func<GetRolesPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return role
            => string.IsNullOrWhiteSpace(search)
            || role.Name.ToNormalize().Contains(search);
    }
}