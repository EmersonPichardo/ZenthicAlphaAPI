using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application._Common.Queries;

public interface IQuery
    : IBaseQuery, IRequest<OneOf<None, Failure>>;

public interface IQuery<TResponse>
    : IBaseQuery, IRequest<OneOf<TResponse, Failure>>;