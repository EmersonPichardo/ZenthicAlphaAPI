using Application.Failures;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.GetAll;
using Identity.Application.Users.GetPaginated;
using Identity.Domain.User;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Users.GetAll;

internal class GetAllUsersQueryHandler(IIdentityDbContext dbContext)
    : GetAllEntitiesQueryHandler<GetAllUsersQuery, GetAllUsersQueryResponse, User>(dbContext)
    , IRequestHandler<GetAllUsersQuery, OneOf<IList<GetAllUsersQueryResponse>, Failure>>
{
    protected override Expression<Func<User, GetAllUsersQueryResponse>> MapToResponse()
    {
        return user => new GetAllUsersQueryResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Status = user.Status.ToString()
        };
    }
}
