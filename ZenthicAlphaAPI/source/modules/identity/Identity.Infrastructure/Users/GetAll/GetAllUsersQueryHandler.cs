using Application.Failures;
using Identity.Application.Users.GetAll;
using Identity.Domain.User;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Users.GetAll;

internal class GetAllUsersQueryHandler(IdentityModuleDbContext dbContext)
    : GetAllEntitiesQueryHandler<GetAllUsersQuery, GetAllUsersQueryResponse, User>(dbContext)
    , IRequestHandler<GetAllUsersQuery, OneOf<IList<GetAllUsersQueryResponse>, Failure>>
{
    protected override Expression<Func<User, GetAllUsersQueryResponse>> MapToResponse()
    {
        return user => new GetAllUsersQueryResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Status = user.Status.ToString()
        };
    }
}
