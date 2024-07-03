using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Users.ChangePassword;

public interface IChangeUserPasswordCommandHandler
    : IRequestHandler<ChangeUserPasswordCommand, OneOf<None, Failure>>;