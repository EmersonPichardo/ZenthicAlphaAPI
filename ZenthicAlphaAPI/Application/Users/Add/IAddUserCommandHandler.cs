using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Users.Add;

public interface IAddUserCommandHandler
    : IRequestHandler<AddUserCommand, OneOf<None, Failure>>;
