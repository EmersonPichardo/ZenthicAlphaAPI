using Application._Common.Failures;
using MediatR;
using OneOf;

namespace Application.Roles.GetAll;

public interface IGetAllRolesQueryHandler
    : IRequestHandler<GetAllRolesQuery, OneOf<IList<GetAllRolesQueryResponse>, Failure>>;
