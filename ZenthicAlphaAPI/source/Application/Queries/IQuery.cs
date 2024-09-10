using Application.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Queries;

public interface IQuery
    : IBaseQuery, IRequest<OneOf<None, Failure>>;

public interface IQuery<TResponse>
    : IBaseQuery, IRequest<OneOf<TResponse, Failure>>;