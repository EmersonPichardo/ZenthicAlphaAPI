using Application._Common.Failures;
using OneOf;
using OneOf.Types;

namespace Application._Common.Security.Authentication;

public interface ICurrentUserService
{
    Task<OneOf<ICurrentUser, None, Failure>> GetCurrentUserAsync();
}