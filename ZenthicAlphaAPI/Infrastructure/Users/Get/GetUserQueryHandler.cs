using Application._Common.Failures;
using Application._Common.Persistence.Databases;
using Application.Users.Get;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Infrastructure.Users.Get;

internal class GetUserQueryHandler(
    IApplicationDbContext dbContext
)
    : IGetUserQueryHandler
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
