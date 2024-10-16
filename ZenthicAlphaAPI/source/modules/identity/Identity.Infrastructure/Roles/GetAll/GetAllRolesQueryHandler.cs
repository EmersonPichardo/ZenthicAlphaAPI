using Application.Failures;
using Identity.Application.Roles.GetAll;
using Identity.Domain.Roles;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Roles.GetAll;

internal class GetAllRolesQueryHandler(IdentityModuleDbContext dbContext)
    : GetAllEntitiesQueryHandler<GetAllRolesQuery, GetAllRolesQueryResponse, Role>(dbContext)
    , IRequestHandler<GetAllRolesQuery, OneOf<IList<GetAllRolesQueryResponse>, Failure>>
{
    protected override Expression<Func<Role, GetAllRolesQueryResponse>> MapToResponse()
    {
        return role => new GetAllRolesQueryResponse
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}
