namespace Identity.Application._Common.Authentication;

public interface ICurrentUserIdentity
{
    Guid Id { get; init; }
}