using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Roles.Update;

public interface IUpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, OneOf<None, Failure>>;
