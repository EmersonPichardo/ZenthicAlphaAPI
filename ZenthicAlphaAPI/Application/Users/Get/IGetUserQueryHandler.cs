using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Users.Get;

public interface IGetUserQueryHandler
    : IRequestHandler<GetUserQuery, OneOf<GetUserQueryResponse, Failure>>;
