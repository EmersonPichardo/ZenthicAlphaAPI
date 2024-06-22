using MediatR;

namespace Application.Users.GetAll;

public interface IGetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, IList<GetAllUsersQueryResponse>>;
