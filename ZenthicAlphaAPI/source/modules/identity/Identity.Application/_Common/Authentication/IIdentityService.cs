using Application.Failures;
using OneOf;
using OneOf.Types;

namespace Identity.Application._Common.Authentication;

public interface IIdentityService
{
    bool IsCurrentUserAuthenticated();
    bool IsCurrentUserNotAuthenticated() => !IsCurrentUserAuthenticated();
    OneOf<ICurrentUserIdentity, None, Failure> GetCurrentUserIdentity();
    bool IsRefreshTokenCaller();
    bool IsNotRefreshTokenCaller() => !IsRefreshTokenCaller();
}