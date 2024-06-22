using Domain.Security;

namespace Application.Users;

public record UserDto : IMapFrom<User>
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; }
}
