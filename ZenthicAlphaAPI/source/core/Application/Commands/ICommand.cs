﻿using Application.Failures;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Application.Commands;

public interface ICommand
    : IBaseCommand, IRequest<OneOf<Success, Failure>>;

public interface ICommand<TResponse>
    : IBaseCommand, IRequest<OneOf<TResponse, Failure>>;