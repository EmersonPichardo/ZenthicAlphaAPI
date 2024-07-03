using Application._Common.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application._Common.Commands;

public interface ICommand
    : IBaseCommand, IRequest<OneOf<None, Failure>>;

public interface ICommand<TResponse>
    : IBaseCommand, IRequest<OneOf<TResponse, Failure>>;