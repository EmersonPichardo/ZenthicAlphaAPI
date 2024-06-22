using Application._Common.Helpers;
using Application._Common.Persistence.Databases;
using Application.Roles.GetPaginated;
using AutoMapper;
using Domain.Security;
using Infrastructure._Common.GenericHandlers;
using System.Linq.Expressions;

namespace Infrastructure.Roles.GetPaginated;

internal class GetRolesPaginatedQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : GetEntitiesPaginatedQueryHandler<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse, Role>(dbContext, mapper, GetFilterExpression)
    , IGetRolesPaginatedQueryHandler
{
    private static Expression<Func<GetRolesPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return role
            => string.IsNullOrWhiteSpace(search)
            || role.Name.ToNormalize().Contains(search);
    }
}