using Application.Failures;
using OneOf;
using OneOf.Types;

namespace Identity.Application._Common.Authentication;

public interface ICurrentUserService
{
    Task<OneOf<ICurrentUser, None, Failure>> GetCurrentUserAsync();
}