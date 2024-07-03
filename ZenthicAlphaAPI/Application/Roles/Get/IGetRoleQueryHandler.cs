using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Roles.Get;

public interface IGetRoleQueryHandler
    : IRequestHandler<GetRoleQuery, OneOf<GetRoleQueryResponse, Failure>>;
