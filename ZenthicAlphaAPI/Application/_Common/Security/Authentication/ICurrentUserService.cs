namespace Application._Common.Security.Authentication;

public interface ICurrentUserService
{
    Task<ICurrentUser?> GetCurrentUserAsync();
}