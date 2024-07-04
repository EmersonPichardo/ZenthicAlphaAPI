using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Users.GetAll;

public interface IGetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, OneOf<IList<GetAllUsersQueryResponse>, Failure>>;
