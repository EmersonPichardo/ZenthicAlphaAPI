using MediatR;

namespace Application.Users.Get;

public interface IGetUserQueryHandler
    : IRequestHandler<GetUserQuery, GetUserQueryResponse>;
