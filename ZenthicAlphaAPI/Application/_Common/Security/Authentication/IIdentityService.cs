using Application._Common.Failures;
using OneOf;
using OneOf.Types;

namespace Application._Common.Security.Authentication;

public interface IIdentityService
{
    bool IsCurrentUserAuthenticated();
    bool IsCurrentUserNotAuthenticated() => !IsCurrentUserAuthenticated();
    OneOf<ICurrentUserIdentity, None, Failure> GetCurrentUserIdentity();
    bool IsRefreshTokenCaller();
    bool IsNotRefreshTokenCaller() => !IsRefreshTokenCaller();
}