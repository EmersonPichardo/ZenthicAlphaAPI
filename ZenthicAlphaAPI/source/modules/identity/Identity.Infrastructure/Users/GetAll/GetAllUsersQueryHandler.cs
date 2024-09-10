using Application.Failures;
using AutoMapper;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Users.GetAll;
using Identity.Domain.User;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Users.GetAll;

internal class GetAllUsersQueryHandler(IIdentityDbContext dbContext, IMapper mapper)
    : GetAllEntitiesQueryHandler<GetAllUsersQuery, GetAllUsersQueryResponse, User>(dbContext, mapper)
    , IRequestHandler<GetAllUsersQuery, OneOf<IList<GetAllUsersQueryResponse>, Failure>>;
