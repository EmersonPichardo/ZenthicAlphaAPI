namespace Application.Auth;

public interface IUserSessionService
{
    Task<IUserSession> GetSessionAsync();
}
