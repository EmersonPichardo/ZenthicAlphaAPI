using MediatR;

namespace Application._Common.Commands;

public interface ICommand
    : IBaseCommand, IRequest;

public interface ICommand<out TResponse>
    : IBaseCommand, IRequest<TResponse>;