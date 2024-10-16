namespace Application.Auth;

public interface IUserSessionInfo
{
    IUserSession Session { get; init; }
}
