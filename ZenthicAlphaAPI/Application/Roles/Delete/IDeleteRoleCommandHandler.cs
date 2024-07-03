using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Roles.Delete;

public interface IDeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, OneOf<None, Failure>>;
