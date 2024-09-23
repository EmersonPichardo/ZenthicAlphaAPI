using Application.Failures;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Identity.Application.Roles.Add;

public interface IAddRoleCommandHandler
    : IRequestHandler<AddRoleCommand, OneOf<None, Failure>>;
