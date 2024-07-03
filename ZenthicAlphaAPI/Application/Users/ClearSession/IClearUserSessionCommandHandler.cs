using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Users.ClearSession;

public interface IClearUserSessionCommandHandler
    : IRequestHandler<ClearUserSessionCommand, OneOf<None, Failure>>;
