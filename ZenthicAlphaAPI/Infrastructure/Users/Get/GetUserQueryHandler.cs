using Application._Common.Exceptions;
using Application._Common.Persistence.Databases;
using Application.Users.Get;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Get;

internal class GetUserQueryHandler(
    IApplicationDbContext dbContext
)
    : IGetUserQueryHandler
{
    public async Task<GetUserQueryResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .FirstOrDefaultAsync(
                user => user.Id.Equals(request.Id),
                cancellationToken
            )
        ?? throw new NotFoundException(nameof(dbContext.Roles), request.Id);

        return new()
        {
            Id = foundUser.Id,
            FullName = foundUser.FullName,
            Email = foundUser.Email,
            Status = foundUser.Status.ToString()
        };
    }
}
