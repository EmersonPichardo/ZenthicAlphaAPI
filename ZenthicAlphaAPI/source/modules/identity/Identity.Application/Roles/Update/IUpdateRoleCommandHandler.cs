using Application.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Identity.Application.Roles.Update;

public interface IUpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, OneOf<None, Failure>>;
