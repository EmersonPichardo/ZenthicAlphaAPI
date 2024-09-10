using Application.Failures;
using AutoMapper;
using Identity.Application._Common.Persistence.Databases;
using Identity.Application.Roles.GetAll;
using Identity.Domain.Roles;
using Infrastructure.GenericHandlers;
using MediatR;
using OneOf;

namespace Identity.Infrastructure.Roles.GetAll;

internal class GetAllRolesQueryHandler(IIdentityDbContext dbContext, IMapper mapper)
    : GetAllEntitiesQueryHandler<GetAllRolesQuery, GetAllRolesQueryResponse, Role>(dbContext, mapper)
    , IRequestHandler<GetAllRolesQuery, OneOf<IList<GetAllRolesQueryResponse>, Failure>>;
