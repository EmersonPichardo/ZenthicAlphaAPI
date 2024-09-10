using Application.Failures;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.Get;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Identity.Infrastructure.Users.Get;

internal class GetUserQueryHandler(
    IIdentityDbContext dbContext
)
    : IRequestHandler<GetUserQuery, OneOf<GetUserQueryResponse, Failure>>
{
    public async Task<OneOf<GetUserQueryResponse, Failure>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .FirstOrDefaultAsync(
                user => user.Id.Equals(request.Id),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.NotFound("User not found", $"No user was found with an Id of {request.Id}");

        return new GetUserQueryResponse()
        {
            Id = foundUser.Id,
            FullName = foundUser.FullName,
            Email = foundUser.Email,
            Status = foundUser.Status.ToString()
        };
    }
}
