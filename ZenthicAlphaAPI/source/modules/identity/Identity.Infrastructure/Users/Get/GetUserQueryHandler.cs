using Application.Failures;
using Identity.Application.Users.Get;
using Identity.Infrastructure.Persistence.Databases.IdentityDbContext;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Users.Get;

internal class GetUserQueryHandler(
    IdentityModuleDbContext dbContext
)
    : IRequestHandler<GetUserQuery, OneOf<GetUserQueryResponse, Failure>>
{
    public async Task<OneOf<GetUserQueryResponse, Failure>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .FindAsync([request.Id], cancellationToken);

        if (foundUser is null)
            return FailureFactory.NotFound("User not found", $"No user was found with an Id of {request.Id}");

        return new GetUserQueryResponse
        {
            Id = foundUser.Id,
            UserName = foundUser.UserName,
            Email = foundUser.Email,
            Status = foundUser.Status.ToString()
        };
    }
}
