using MediatR;

namespace Application._Common.Queries;

public interface IQuery
    : IBaseQuery, IRequest;

public interface IQuery<out TResponse>
    : IBaseQuery, IRequest<TResponse>;