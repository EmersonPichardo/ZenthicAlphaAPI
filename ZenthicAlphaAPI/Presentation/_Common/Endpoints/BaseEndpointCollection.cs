using Application._Common.Commands;
using Application._Common.Pagination;
using Application._Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation._Common.ExceptionHandler;
using System.Collections.ObjectModel;

namespace Presentation._Common.Endpoints;

public abstract class BaseEndpointCollection(
    string collectionName
)
    : IEndpointCollection
{
    protected string CollectionName { get; } = collectionName
        ?? throw new ArgumentNullException(collectionName);

    private readonly Collection<Endpoint> endpoints = [];

    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"api/{CollectionName}");

        foreach (var endpoint in endpoints)
        {
            var endpointBuilder = endpoint.Verbose switch
            {
                HttpVerbose.Get => group.MapGet(endpoint.Route, endpoint.Handler),
                HttpVerbose.Post => group.MapPost(endpoint.Route, endpoint.Handler),
                HttpVerbose.Put => group.MapPut(endpoint.Route, endpoint.Handler),
                HttpVerbose.Patch => group.MapPatch(endpoint.Route, endpoint.Handler),
                HttpVerbose.Delete => group.MapDelete(endpoint.Route, endpoint.Handler),
                _ => throw new NotImplementedException()
            };

            endpointBuilder
                .AllowAnonymous()
                .WithTags($"{CollectionName}EndpointCollection")
                .Produces(endpoint.SuccessStatusCode, endpoint.SuccessType)
                .Produces(401, typeof(ProblemDetails))
                .Produces(403, typeof(ProblemDetails))
                .Produces(500, typeof(ProblemDetails));
        }
    }

    protected void DefineEndpoint(
        HttpVerbose verbose,
        string route,
        Delegate handler,
        int successStatusCode,
        Type? successType = null
    )
    {
        endpoints.Add(
            new(
                verbose,
                route,
                handler,
                successStatusCode,
                successType
            )
        );
    }

    protected void DefineGetPaginatedEndpoint<TQuery, TResponse>()
        where TQuery : GetEntitiesPaginatedQuery<TResponse>, new()
        where TResponse : class
    {
        DefineEndpoint(HttpVerbose.Get, "/paginated",
            GetPaginated, 200, typeof(PaginatedList<TResponse>));

        static async Task<IResult> GetPaginated(ISender mediator, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? search)
        {
            var query = new TQuery()
            {
                Search = search,
                CurrentPage = page,
                PageSize = size
            };

            var result = await mediator.Send(query);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineGetAllEndpoint<TQuery, TResponse>()
        where TQuery : GetAllEntitiesQuery<TResponse>, new()
        where TResponse : class
    {
        DefineEndpoint(HttpVerbose.Get, "/",
            GetAll, 200, typeof(IList<TResponse>));

        static async Task<IResult> GetAll(ISender mediator)
        {
            var query = new TQuery();
            var result = await mediator.Send(query);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineGetEndpoint<TQuery, TResponse>(string? route = null)
        where TQuery : GetEntityQuery<TResponse>, new()
        where TResponse : class
    {
        DefineEndpoint(HttpVerbose.Get, route ?? "/{id:guid}",
            Get, 200, typeof(TResponse));

        static async Task<IResult> Get(ISender mediator, Guid id)
        {
            var query = new TQuery() { Id = id };
            var result = await mediator.Send(query);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineInsertEndpoint<TCommand>()
        where TCommand : ICommand
    {
        DefineEndpoint(HttpVerbose.Post, "/",
            Insert, 201);

        static async Task<IResult> Insert(ISender mediator, TCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                ResultFactory.Created,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineInsertEndpoint<TCommand, TResponse>()
        where TCommand : ICommand<TResponse>
    {
        DefineEndpoint(HttpVerbose.Post, "/",
            Insert, 201);

        static async Task<IResult> Insert(ISender mediator, TCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                ResultFactory.Created,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineUpdateEndpoint<TCommand>()
        where TCommand : ICommand
    {
        DefineEndpoint(HttpVerbose.Put, "/",
            Update, 200);

        static async Task<IResult> Update(ISender mediator, TCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineUpdateEndpoint<TCommand, TResponse>()
        where TCommand : ICommand<TResponse>
    {
        DefineEndpoint(HttpVerbose.Put, "/",
            Update, 200);

        static async Task<IResult> Update(ISender mediator, TCommand command)
        {
            var result = await mediator.Send(command);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
    protected void DefineDeleteEndpoint<TCommand>()
        where TCommand : IDeleteCommand, new()
    {
        DefineEndpoint(HttpVerbose.Delete, "/{id:guid}",
            Delete, 200);

        static async Task<IResult> Delete(ISender mediator, Guid id)
        {
            var command = new TCommand() { Id = id };
            var result = await mediator.Send(command);

            return result.Match(
                ResultFactory.Ok,
                ResultFactory.ProblemDetails
            );
        }
    }
}
