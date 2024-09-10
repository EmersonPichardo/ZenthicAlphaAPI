using Application.Events;
using Application.Failures;
using Identity.Application._Common.Authentication;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application._Common.Settings;
using Identity.Application.Users.ClearSession;
using Identity.Application.Users.Login;
using Identity.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Identity.Infrastructure.Users.Login;

internal class LoginUserCommandHandler(
    IIdentityDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    ISender mediator,
    IEventPublisher eventPublisher
)
    : IRequestHandler<LoginUserCommand, OneOf<LoginUserCommandResponse, Failure>>
{
    public async Task<OneOf<LoginUserCommandResponse, Failure>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext
            .Users
            .AsNoTrackingWithIdentityResolution()
            .Include(entity => entity.UserRoles)
                .ThenInclude(entity => entity.Role)
                    .ThenInclude(entity => entity != null ? entity.Permissions : null)
            .AsSingleQuery()
            .SingleOrDefaultAsync(
                user
                    => user.Email.Equals(command.Email)
                    && !user.Status.Equals(UserStatus.Inactive),
                cancellationToken
            );

        if (foundUser is null)
            return FailureFactory.UnauthorizedAccess(detail: "Email incorrecto o contraseña incorrecta");

        var hashingSettings = new HashingSettings()
        {
            Algorithm = foundUser.Algorithm,
            Iterations = foundUser.Iterations
        };

        var hashedPassword = passwordHasher.Hash(command.Password, foundUser.Salt, hashingSettings);

        if (!hashedPassword.Equals(foundUser.Password, StringComparison.Ordinal))
            return FailureFactory.UnauthorizedAccess(detail: "Email incorrecto o contraseña incorrecta");

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
            Status = foundUser.Status.ToString(),
            RefreshToken = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtService.GetSettings().RefreshTokenLifetime),
                Value = jwtService.GenerateJwtRefreshToken(foundUser.Id)
            },
            Token = new()
            {
                ExpirationDate = DateTime.UtcNow.Add(jwtService.GetSettings().TokenLifetime),
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
