using Application._Common.Events;
using Application._Common.Persistence.Databases;
using Application._Common.Security.Authentication;
using Application._Common.Settings;
using Application.Users.ClearSession;
using Application.Users.Login;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Login;

internal class LoginUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    ISender mediator,
    IEventPublisher eventPublisher
)
    : ILoginUserCommandHandler
{
    public async Task<LoginUserCommandResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity != null ? entity.Permissions : null)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                user => user.Email.Equals(command.Email),
                cancellationToken
            )
        ?? throw new UnauthorizedAccessException();

        var hashingSettings = new HashingSettings()
        {
            Algorithm = foundUser.Algorithm,
            Iterations = foundUser.Iterations
        };

        var hashedPassword = passwordHasher.Hash(command.Password, foundUser.Salt, hashingSettings);

        if (!hashedPassword.Equals(foundUser.Password, StringComparison.Ordinal))
            throw new UnauthorizedAccessException();

        var userAccess = foundUser
            .UserRoles
            .Select(entity => entity.Role)
            .SelectMany(entity => entity?.Permissions ?? [])
            .Where(entity
                => entity.RequiredAccess > 0
            )
            .GroupBy(
                entity => entity.Component,
                entity => entity.RequiredAccess
            )
            .ToDictionary(
                entity => entity.Key.ToString(),
                entity => entity.Aggregate((accessLevel, rolePermission) => accessLevel | rolePermission)
            );

        var response = new LoginUserCommandResponse()
        {
            DisplayName = foundUser.FullName,
            Status = foundUser.Status,
            RefreshToken = new()
            {
                Expires = DateTime.UtcNow.Add(jwtService.GetSettings().RefreshTokenLifetime),
                Value = jwtService.GenerateJwtRefreshToken(foundUser.Id)
            },
            Token = new()
            {
                Expires = DateTime.UtcNow.Add(jwtService.GetSettings().TokenLifetime),
                Value = jwtService.GenerateJwtToken(foundUser.Id)
            },
            Access = userAccess
        };

        await mediator.Send(
            new ClearUserSessionCommand() { UserId = foundUser.Id },
            cancellationToken
        );

        eventPublisher.EnqueueEvent(
            new UserLoggedInEvent() { Entity = foundUser, Session = response }
        );

        return response;
    }
}
