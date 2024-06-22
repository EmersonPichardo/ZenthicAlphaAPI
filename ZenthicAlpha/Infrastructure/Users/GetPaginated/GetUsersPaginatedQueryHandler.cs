using Application._Common.Helpers;
using Application._Common.Persistence.Databases;
using Application.Users.GetPaginated;
using AutoMapper;
using Domain.Security;
using Infrastructure._Common.GenericHandlers;
using System.Linq.Expressions;

namespace Infrastructure.Users.GetPaginated;

internal class GetUsersPaginatedQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : GetEntitiesPaginatedQueryHandler<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse, User>(dbContext, mapper, GetFilterExpression)
    , IGetUsersPaginatedQueryHandler
{
    private static Expression<Func<GetUsersPaginatedQueryResponse, bool>> GetFilterExpression(string? search)
    {
        return user
            => string.IsNullOrWhiteSpace(search)
            || user.FullName.ToNormalize().Contains(search)
            || user.Email.ToNormalize().Contains(search);
    }
}