namespace Application._Common.Security.Authentication;

public interface ICurrentUserIdentity
{
    Guid Id { get; init; }
}