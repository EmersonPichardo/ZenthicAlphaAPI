using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Users.ResetPassword;

public interface IResetUserPasswordCommandHandler
    : IRequestHandler<ResetUserPasswordCommand, OneOf<None, Failure>>;
