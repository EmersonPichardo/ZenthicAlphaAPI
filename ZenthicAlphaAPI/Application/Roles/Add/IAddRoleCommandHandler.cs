using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Roles.Add;

public interface IAddRoleCommandHandler
    : IRequestHandler<AddRoleCommand, OneOf<None, Failure>>;
