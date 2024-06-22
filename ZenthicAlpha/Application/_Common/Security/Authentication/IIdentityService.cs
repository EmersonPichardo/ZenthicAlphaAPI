namespace Application._Common.Security.Authentication;

public interface IIdentityService
{
    bool IsCurrentUserAuthenticated();
    bool IsCurrentUserNotAuthenticated() => !IsCurrentUserAuthenticated();
    ICurrentUserIdentity? GetCurrentUserIdentity();
    bool IsRefreshTokenCaller();
    bool IsNotRefreshTokenCaller() => !IsRefreshTokenCaller();
}