using Identity.Domain.User;

namespace Identity.Application.Users;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }

    public static UserDto FromUser(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        Status = user.Status.ToString()
    };
}
